using Microsoft.AspNetCore.Mvc;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;

namespace AutoRefineOpenAI.Controllers;

public class IngestController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly CreateBranchRequest GitHubConfiguration;

    public IngestController(IConfiguration configuration)
    {
        _configuration = configuration;

        this.GitHubConfiguration = new CreateBranchRequest()
        {
            GitHubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN") ?? throw new ArgumentNullException("GITHUB_TOKEN"),
            Owner = _configuration["GitHub:Owner"] ?? throw new ArgumentNullException("GitHub:Owner"),
            Repository = _configuration["GitHub:Repository"] ?? throw new ArgumentNullException("GitHub:Repository"),
            BaseBranch = _configuration["GitHub:BaseBranch"] ?? throw new ArgumentNullException("GitHub:BaseBranch"),
            UserAgent = _configuration["GitHub:UserAgent"] ?? throw new ArgumentNullException("GitHub:UserAgent")
        };
    }

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

    [HttpPost("create-branch")]
    public async Task<IActionResult> CreateBranch([FromBody] string NewBranchName)
    {
        try
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"token {GitHubConfiguration.GitHubToken}");
            httpClient.DefaultRequestHeaders.Add("User-Agent", GitHubConfiguration.UserAgent);

            // Get the default branch reference
            var branchResponse = await httpClient.GetAsync($"https://api.github.com/repos/{GitHubConfiguration.Owner}/{GitHubConfiguration.Repository}/git/ref/heads/{GitHubConfiguration.BaseBranch}");
            if (!branchResponse.IsSuccessStatusCode)
            {
                return BadRequest("Failed to fetch the base branch.");
            }

            var branchData = await branchResponse.Content.ReadFromJsonAsync<GitHubBranchResponse>();
            if (branchData == null)
            {
                return BadRequest("Invalid response from GitHub API.");
            }

            // Create a new branch
            var createBranchPayload = new
            {
                @ref = $"refs/heads/{NewBranchName}",
                sha = branchData.Object.Sha
            };

            var createBranchResponse = await httpClient.PostAsJsonAsync($"https://api.github.com/repos/{GitHubConfiguration.Owner}/{GitHubConfiguration.Repository}/git/refs", createBranchPayload);
            if (!createBranchResponse.IsSuccessStatusCode)
            {
                return BadRequest("Failed to create the new branch.");
            }

            return Ok("Branch created successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    public class GitHubBranchResponse
    {
        public GitHubBranchObject Object { get; set; }
    }

    public class GitHubBranchObject
    {
        public string Sha { get; set; }
    }
}

public class MixpanelEvent
{
    public required string Event { get; set; }
    public Dictionary<string, object> Properties { get; set; }
}

public class CreateBranchRequest
{
    public required string GitHubToken { get; set; }
    public required string Owner { get; set; }
    public required string Repository { get; set; }
    public required string BaseBranch { get; set; }
    public required string UserAgent { get; set; }
}