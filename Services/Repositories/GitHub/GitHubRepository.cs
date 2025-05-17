using System;

namespace Services.Repositories.GitHub;

public class GitHubRepository : ARepository
{
    private readonly IConfiguration _configuration;
    private readonly GitHubConfiguration _gitHubConfiguration;

    public GitHubRepository(IConfiguration configuration)
    {
        _configuration = configuration;

        this._gitHubConfiguration = new GitHubConfiguration()
        {
            GitHubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN") ?? throw new ArgumentNullException("GITHUB_TOKEN"),
            Owner = _configuration["GitHub:Owner"] ?? throw new ArgumentNullException("GitHub:Owner"),
            Repository = _configuration["GitHub:Repository"] ?? throw new ArgumentNullException("GitHub:Repository"),
            BaseBranch = _configuration["GitHub:BaseBranch"] ?? throw new ArgumentNullException("GitHub:BaseBranch"),
            UserAgent = _configuration["GitHub:UserAgent"] ?? throw new ArgumentNullException("GitHub:UserAgent")
        };
    }

    public override async Task<RepositoryResponse> CreateBranch(string newBranchName)
    {
        try
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"token {_gitHubConfiguration.GitHubToken}");
            httpClient.DefaultRequestHeaders.Add("User-Agent", _gitHubConfiguration.UserAgent);

            // Get the default branch reference
            var branchResponse = await httpClient.GetAsync($"https://api.github.com/repos/{_gitHubConfiguration.Owner}/{_gitHubConfiguration.Repository}/git/ref/heads/{_gitHubConfiguration.BaseBranch}");
            if (!branchResponse.IsSuccessStatusCode)
            {
                return new RepositoryResponse { StatusCode = (int)branchResponse.StatusCode, Message = "Failed to fetch the base branch." };
            }

            var branchData = await branchResponse.Content.ReadFromJsonAsync<GitHubBranchResponse>();
            if (branchData == null)
            {
                return new RepositoryResponse { StatusCode = 400, Message = "Invalid response from GitHub API." };
            }

            // Create a new branch
            var createBranchPayload = new
            {
                @ref = $"refs/heads/{newBranchName}",
                sha = branchData.Object.Sha
            };

            var createBranchResponse = await httpClient.PostAsJsonAsync($"https://api.github.com/repos/{_gitHubConfiguration.Owner}/{_gitHubConfiguration.Repository}/git/refs", createBranchPayload);
            if (!createBranchResponse.IsSuccessStatusCode)
            {
                return new RepositoryResponse { StatusCode = (int)createBranchResponse.StatusCode, Message = "Failed to create the new branch." };
            }

            return new RepositoryResponse { StatusCode = 0, Message = "Branch created successfully." };
        }
        catch (Exception ex)
        {
            return new RepositoryResponse { StatusCode = 500, Message = $"Internal server error: {ex.Message}" };
        }
    }

    public async override Task<RepositoryResponse> CreatePullRequest(string branchName, string baseBranch, string body)
    {
        try
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"token {_gitHubConfiguration.GitHubToken}");
            httpClient.DefaultRequestHeaders.Add("User-Agent", _gitHubConfiguration.UserAgent);

            var pullRequestPayload = new
            {
                title = $"Pull Request for {branchName} to {baseBranch}",
                head = branchName,
                @base = baseBranch,
                body = body
            };

            var pullRequestResponse = await httpClient.PostAsJsonAsync(
                $"https://api.github.com/repos/{_gitHubConfiguration.Owner}/{_gitHubConfiguration.Repository}/pulls",
                pullRequestPayload);

            if (!pullRequestResponse.IsSuccessStatusCode)
            {
                return new RepositoryResponse { StatusCode = 400, Message = "Failed to create the pull request." };
            }

            var pullRequestData = await pullRequestResponse.Content.ReadAsStringAsync();
            return new RepositoryResponse { StatusCode = 0, Message = $"Pull request created successfully: {pullRequestData}" };
        }
        catch (Exception ex)
        {
            return new RepositoryResponse { StatusCode = 500, Message = $"Internal server error: {ex.Message}" };
        }
    }
    
    public class GitHubConfiguration
    {
        public required string GitHubToken { get; set; }
        public required string Owner { get; set; }
        public required string Repository { get; set; }
        public required string BaseBranch { get; set; }
        public required string UserAgent { get; set; }
    }
}
