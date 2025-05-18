using System.Text;
using System.Text.Json;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using OpenAI.Chat;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace OpenAILambda;

public class Function
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public Function(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<string> FunctionHandler(string input, ILambdaContext context)
    {
        var githubService = new GithubService(_configuration);

        var files = await githubService.GetRepositoryFilesAsync();

        var result = await AnalyzeCodeAndLogs(files, await File.ReadAllTextAsync("logs.json"));
        
        /*
        var mixPanelUrl = "https://api.mixpanel.com/engage/";
        
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(mixPanelUrl);
        var mixpanelResponse = await httpClient.PostAsync("api/retrieve-logs", new StringContent(JsonSerializer.Serialize(new
        {
            startDate = DateTime.Now,
            endDate = DateTime.Now.AddDays(1),
            eventName = "event_name",
        })));
        
        if (!mixpanelResponse.IsSuccessStatusCode)
        {
            context.Logger.LogLine($"Error retrieving logs: {mixpanelResponse.ReasonPhrase}");
            return "Error retrieving logs";
        }

        var logs = JsonSerializer.Deserialize<dynamic>(await mixpanelResponse.Content.ReadAsStringAsync());
        
        var prompt = "Suggest improvement or actions to take based on the following mixpanel logs." +
                     $" {logs}" +
                     " The action needs to be concise. For example: change regex, remove field, add hint.";

        var d = new OpenAIClient("");
        var f =d.GetOpenAIFileClient();
        var ms = ChatMessage.CreateUserMessage("");
        var client = new ChatClient(model: "gpt-4o", apiKey: "");

        var completion = await client.CompleteChatAsync(prompt);*/
        
        //do something
        
        return "All Good Here!";
    }
    
    private async Task<List<dynamic>> AnalyzeCodeAndLogs(List<RepositoryFile> files, string logs)
    {
        var analysisResults = new List<dynamic>();

        try
        {
            foreach (var file in files)
            {
                if (!file.Path.EndsWith(".html") && !file.Path.EndsWith(".js") && !file.Path.EndsWith(".ts") && !file.Path.EndsWith(".tsx"))
                {
                    continue;
                }

                var messages = new List<ChatMessage>
                {
                    ChatMessage.CreateSystemMessage(@"I want you to act as an expert assistant in software development and code review.
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

                messages.Add(ChatMessage.CreateUserMessage(@"Please analyze the file and return the entire updated file in JSON format with the following structure:
                {
                    'filepath': '<file_path>',
                    'lines': [
                        { 'lineNumber': <line_number>, 'currentValue': '<current_line_content>', 'newValue': '<new_line_content>' },
                        ...
                    ]
                }
                Ensure that each line in the 'lines' array corresponds to the actual line number in the file, and the 'currentValue' matches the original content of the line, while 'newValue' contains the updated content if any changes are suggested. If no changes are needed for a line, 'newValue' should be the same as 'currentValue'."));

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
