using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;
using Services.Helpers;
using Services.Models;
using Services.Repositories;

namespace Services.Services;

[Route("api/[controller]")]
[ApiController]
public class OpenAiController(GithubService githubService, PlaywrightService playwrightService) : ControllerBase
{
    
    [HttpPost("analyze")]
    public async Task<string> FunctionHandler()
    {
        var files = await githubService.GetRepositoryFilesAsync();

        var logs = await playwrightService.RetrieveLogs(new MonitoringControllerService.DateRange
        {
            StartDate = DateTime.Now.AddDays(-1),
            EndDate = DateTime.Now,
            EventName = "event_name"
        });
        
        var result = await AnalyzeCodeAndLogs(files, JsonSerializer.Serialize(logs));

        return "All Good Here!";
    }
    
    private async Task<List<dynamic>> AnalyzeCodeAndLogs(List<RepositoryFile> files, string logs)
    {
        var analysisResults = new List<dynamic>();

        try
        {
            foreach (var file in files)
            {
                if (!file.Path.EndsWith(".html") && !file.Path.EndsWith(".js") && !file.Path.EndsWith(".ts") &&
                    !file.Path.EndsWith(".tsx"))
                {
                    continue;
                }

                var messages = new List<ChatMessage>
                {
                    ChatMessage.CreateSystemMessage(
                        @"I want you to act as an expert assistant in software development and code review.
                    You have access to a GitHub repository and can analyze source code files in a workspace.
                    When I provide you with a specific file, do the following:
                    Analyze the file to identify potential issues and security vulnerabilities.
                    Propose a detailed solution.
                    Generate the exact code snippet that needs to be replaced or added.")
                };

                var fileContent = $"## FILE: {file.Path}\n```\n{file.Content}\n```";
                messages.Add(ChatMessage.CreateUserMessage(fileContent));

                if (!string.IsNullOrEmpty(logs))
                {
                    string trimmedLogs = logs.Length > 50000 ? logs.Substring(0, 50000) + "... [LIMIT REACHED]" : logs;
                    messages.Add(ChatMessage.CreateUserMessage($"# LOGS\n```\n{trimmedLogs}\n```"));
                }

                messages.Add(ChatMessage.CreateUserMessage(
                    @"Please analyze the file. Return a JSON object containing:

""reason"": A brief summary of why the change is suggested (e.g., bug fix, refactoring, feature enhancement).

""file_path"": Relative path of the file.

""line_range"": Approximate line number range where the change occurred (e.g., ""L25-L35"").

""before"": The old file code.

""after"": The new file code."));

                var apiKey = Environment.GetEnvironmentVariable("CHATGPT_API_KEY");
                var client = new ChatClient(model: "gpt-4o", apiKey: apiKey);
                var result = await client.CompleteChatAsync(messages);

                if (result != null && result.Value != null && result.Value.Content.Count > 0)
                {
                    var analysisResult = result.Value.Content[0].Text;
                    analysisResults.Add(analysisResult);
                }
            }
        }
        catch (Exception ex)
        {
            analysisResults.Add(new
            {
                Error = $"Error analyzing file: {ex.Message}"
            });
        }

        return analysisResults;
    }
}