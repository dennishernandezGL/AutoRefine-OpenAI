using Microsoft.AspNetCore.Mvc;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;

namespace AutoRefineOpenAI.Controllers;

public class IngestController : ControllerBase
{
    [HttpPost("ingest")]
    public async Task<IActionResult> Ingest([FromBody] MixpanelEvent @event)
    {
        var prompt = $"Mixpanel event “{@event.Event}” " +
                     $"with properties: {string.Join(", ", @event.Properties.Select(kv => $"{kv.Key}={kv.Value}"))}." +
                     " Suggest improvement or actions to take. The action needs to be concise. For example: change regex, remove field, add hint. Return a JSON format {name: string, description: string, action: string}.";

        var client = new ChatClient(model: "gpt-4o", apiKey: "");

        var completion = await client.CompleteChatAsync(prompt);
        
        return Ok(completion.Value.Content[0].Text);
    }
}

public class MixpanelEvent
{
    public required string Event { get; set; }
    public Dictionary<string, object> Properties { get; set; }
}