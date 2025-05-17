using System.Text.Json;
using Amazon.Lambda.Core;
using OpenAI.Chat;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace OpenAILambda;

public class Function
{
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<string> FunctionHandler(string input, ILambdaContext context)
    {
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

        var client = new ChatClient(model: "gpt-4o", apiKey: "");

        var completion = await client.CompleteChatAsync(prompt);
        
        //do something
        
        return "All Good Here!";
    }
}
