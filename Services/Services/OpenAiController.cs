using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;
using Services.Helpers;
using Services.Logging;
using Services.Models;
using Services.Repositories;

namespace Services.Services;

[Route("api/[controller]")]
[ApiController]
public class OpenAiController(IConfiguration configuration) : ControllerBase
{
    
    [HttpPost("analyze")]
    public async Task<IActionResult> FunctionHandler()
    {
        var repoService = ARepository.Create(Repositories.Repositories.GitHub, configuration);
        var loggingService = LoggingFacade.GetInstance(LoggingConnections.MixPanel, configuration);
        var files = await repoService.GetRepositoryFilesAsync();

        var logs = await loggingService.RetrieveLogsAsync(DateTime.Now.AddDays(-1), DateTime.Now, "Info");
        
        var result = await AnalyzeCodeAndLogs(files, JsonSerializer.Serialize(logs));

        var workspaceRepository = ARepository.Create(Repositories.Repositories.GitHub, configuration);
        var branchName = $"Ai-Analysis-{DateTime.Now:yyyyMMdd-HHmmss}";
        var branchResponse = await workspaceRepository.CreateBranch(branchName);
        var fileChanges = new Dictionary<string, List<string>>();
        foreach (var analysisResult in result)
        {
            if (!analysisResult.IsSuccess)
            {
                continue;
            }

            var filePath = analysisResult.FilePath;
            var changes = new List<string>
            {
                analysisResult.Before,
                analysisResult.After
            };

            fileChanges[filePath] = changes;
        }
        try
        {
            var commitResponse = await workspaceRepository.CommitChanges(fileChanges, "AI Analysis", branchName);
            if (commitResponse.StatusCode != 0)
            {
                return BadRequest($"Failed to commit changes: {commitResponse.Message}");
            }

            var pullRequestResponse = await workspaceRepository.CreatePullRequest(branchName, "main", BuildPullRequestBody(result));
            if (pullRequestResponse.StatusCode != 0)
            {
                return BadRequest($"Failed to create pull request: {pullRequestResponse.Message}");
            }
        }
        catch (Exception ex)
        {
            return BadRequest($"An error occurred while processing: {ex.Message}");
        }
        
        return Ok("All Good Here!");
    }

    private string BuildPullRequestBody(List<ResultOpenAi> results)
    {
        var sb = new StringBuilder();
        foreach (var result in results)
        {
            sb.AppendLine($"### File: {result.FilePath}");
            sb.AppendLine($"**Reason:** {result.Reason}");
            sb.AppendLine("<br><br>");
        }

        return sb.ToString();
    }
    
    private async Task<List<ResultOpenAi>> AnalyzeCodeAndLogs(List<RepositoryFile> files, string logs)
    {
        var analysisResults = new List<ResultOpenAi>();

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
                        """
                        I want you to act as an expert assistant in software development and code review.
                                            You have access to a MixPanel logs that provide how an user interacts with the form.
                                            When I provide you with a specific file, do the following:
                                            Analyze the file to identify potential issues and security vulnerabilities based on the logs.
                                            Propose a detailed solution and add if the reason is based on potential issue or security issue or based on logs.
                                            Generate the exact code snippet that needs to be replaced or added.
                        """)
                };

                var fileContent = $"## FILE: {file.Path}\n```\n{file.Content}\n```";
                messages.Add(ChatMessage.CreateUserMessage(fileContent));

                if (!string.IsNullOrEmpty(logs))
                {
                    string trimmedLogs = logs.Length > 50000 ? logs.Substring(0, 50000) + "... [LIMIT REACHED]" : logs;
                    messages.Add(ChatMessage.CreateUserMessage($"# LOGS\n```\n{trimmedLogs}\n```"));
                }

                messages.Add(ChatMessage.CreateUserMessage(
                    @"Please analyze the file. Return a valid JSON string object containing:
                            ""reason"": A brief summary of why the change is suggested (e.g., bug fix, refactoring, feature enhancement).
                            ""file_path"": Relative path of the file.
                            ""before"": The old file code.
                            ""after"": The new file code."));

                var apiKey = Environment.GetEnvironmentVariable("CHATGPT_API_KEY");
                var client = new ChatClient(model: "gpt-4o", apiKey: apiKey);
                var result = await client.CompleteChatAsync(messages);

                if (result is { Value: not null } && result.Value.Content.Count > 0)
                {
                    var rawAnalysisResult = result.Value.Content[0].Text;

                    try
                    {
                        // Remove the surrounding ```json and ``` markers to extract the JSON content
                        var jsonStartIndex = rawAnalysisResult.IndexOf("{");
                        var jsonEndIndex = rawAnalysisResult.LastIndexOf("}");
                        if (jsonStartIndex >= 0 && jsonEndIndex > jsonStartIndex)
                        {
                            var jsonContent = rawAnalysisResult.Substring(jsonStartIndex, jsonEndIndex - jsonStartIndex + 1);

                            var cleanedJsonContent = new StringBuilder();
                            var insideString = false;

                            foreach (var ch in jsonContent)
                            {
                                if (ch == '\"') insideString = !insideString;
                                if (!insideString && (ch == '\n' || ch == '\r')) continue;
                                cleanedJsonContent.Append(ch);
                            }

                            using var document = JsonDocument.Parse(cleanedJsonContent.ToString());
                            var analysisResult = JsonSerializer.Deserialize<ResultOpenAi>(cleanedJsonContent.ToString());
                            analysisResults.Add(analysisResult);
                        }
                    }
                    catch (JsonException ex)
                    {
                        //Ignored
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Ignored
        }

        return analysisResults;
    }
}

public class ResultOpenAi
{
    [JsonPropertyName("reason")]
    public string Reason { get; set; }
    [JsonPropertyName("file_path")]
    public string FilePath { get; set; }
    [JsonPropertyName("before")]
    public string Before { get; set; }
    [JsonPropertyName("after")]
    public string After { get; set; }
    [JsonIgnore]
    public bool IsSuccess  =>
        !string.IsNullOrEmpty(Reason) && !string.IsNullOrEmpty(FilePath) && !string.IsNullOrEmpty(Before) &&
        !string.IsNullOrEmpty(After);
}