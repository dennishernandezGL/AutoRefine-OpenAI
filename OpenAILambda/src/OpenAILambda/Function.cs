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
    
    private async Task<string> AnalyzeCodeAndLogs(List<RepositoryFile> files, string logs)
    {
        try
        {
            var messages = new List<ChatMessage>
            {
                ChatMessage.CreateSystemMessage(@"I want you to act as an expert assistant in software development and code review.
                You have access to a GitHub repository and can analyze source code files in a workspace.
                When I provide you with a specific error or error message, do the following:
                Analyze the relevant code to identify the root cause of the error, indicating the file and the affected line(s).
                Propose a detailed solution.
                Generate the exact code snippet that needs to be replaced or added.")
            };

            var codeContent = new StringBuilder();
            codeContent.AppendLine("# CODE FILES\n");
            
            foreach (var file in files)
            {
                if (codeContent.Length > 100000)
                {
                    codeContent.AppendLine("... [LIMIT REACHED] ...");
                    break;
                }
                
                codeContent.AppendLine($"## FILE: {file.Path}\n```\n{file.Content}\n```\n");
            }
            
            messages.Add(ChatMessage.CreateUserMessage(codeContent.ToString()));
            
            if (!string.IsNullOrEmpty(logs))
            {
                string trimmedLogs = logs.Length > 50000 ? logs.Substring(0, 50000) + "... [LIMIT REACHED]" : logs;
                messages.Add(ChatMessage.CreateUserMessage( $"# LOGS\n```\n{trimmedLogs}\n```"));
            }

            messages.Add(ChatMessage.CreateUserMessage( @"Please return only the code snippet with reason of the change, filepath, physical file lines (same as file editor), and code change, in json format."));

            var client = new ChatClient(model: "gpt-4o", apiKey: "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");
            var result = await client.CompleteChatAsync(messages);
            return result.Value.Content[0].Text;
        }
        catch (Exception ex)
        {
            return $"Error al analizar el código con OpenAI: {ex.Message}";
        }
    }
}
