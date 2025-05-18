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
                    @"Please analyze the file. Return a valid JSON string object containing:
""reason"": A brief summary of why the change is suggested (e.g., bug fix, refactoring, feature enhancement).
""file_path"": Relative path of the file.
""before"": The old file code.
""after"": The new file code."));

                var apiKey = Environment.GetEnvironmentVariable("CHATGPT_API_KEY");
                var client = new ChatClient(model: "gpt-4o", apiKey: apiKey);
                var result = await client.CompleteChatAsync(messages);

                if (result != null && result.Value != null && result.Value.Content.Count > 0)
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

                            // Preserve line breaks inside JSON values while removing unnecessary ones outside
                            var cleanedJsonContent = new StringBuilder();
                            bool insideString = false;

                            foreach (var ch in jsonContent)
                            {
                                if (ch == '\"') insideString = !insideString;
                                if (!insideString && (ch == '\n' || ch == '\r')) continue;
                                cleanedJsonContent.Append(ch);
                            }

                            // Parse the cleaned JSON content to ensure it's valid
                            using var document = JsonDocument.Parse(cleanedJsonContent.ToString());
                            var analysisResult = JsonSerializer.Deserialize<dynamic>(cleanedJsonContent.ToString());
                            analysisResults.Add(analysisResult);
                        }
                        else
                        {
                            analysisResults.Add(new
                            {
                                Error = "Invalid JSON format in analysis result."
                            });
                        }
                    }
                    catch (JsonException ex)
                    {
                        analysisResults.Add(new
                        {
                            Error = $"JSON parsing error: {ex.Message}"
                        });
                    }
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