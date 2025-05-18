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

    public async override Task<RepositoryResponse> CommitChanges(Dictionary<string, List<(int Line, string Change)>> fileChanges, string commitMessage, string branchName)
    {
        try
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"token {_gitHubConfiguration.GitHubToken}");
            httpClient.DefaultRequestHeaders.Add("User-Agent", _gitHubConfiguration.UserAgent);

            // Get the latest commit from the base branch
            var branchResponse = await httpClient.GetAsync($"https://api.github.com/repos/{_gitHubConfiguration.Owner}/{_gitHubConfiguration.Repository}/git/ref/heads/{branchName}");
            if (!branchResponse.IsSuccessStatusCode)
            {
                return new RepositoryResponse { StatusCode = (int)branchResponse.StatusCode, Message = "Failed to fetch the base branch." };
            }

            var branchData = await branchResponse.Content.ReadFromJsonAsync<GitHubBranchResponse>();
            if (branchData == null)
            {
                return new RepositoryResponse { StatusCode = 400, Message = "Invalid response from GitHub API." };
            }

            // Get the tree of the latest commit
            var commitResponse = await httpClient.GetAsync($"https://api.github.com/repos/{_gitHubConfiguration.Owner}/{_gitHubConfiguration.Repository}/git/commits/{branchData.Object.Sha}");
            if (!commitResponse.IsSuccessStatusCode)
            {
                return new RepositoryResponse { StatusCode = (int)commitResponse.StatusCode, Message = "Failed to fetch the latest commit." };
            }

            var commitData = await commitResponse.Content.ReadFromJsonAsync<GitHubCommitResponse>();
            if (commitData == null)
            {
                return new RepositoryResponse { StatusCode = 400, Message = "Invalid response from GitHub API." };
            }

            // Fetch the current content of each file and apply changes
            var newTreeItems = new List<object>();
            foreach (var fileChange in fileChanges)
            {
                var filePath = fileChange.Key;

                // Fetch the file content
                var fileResponse = await httpClient.GetAsync($"https://api.github.com/repos/{_gitHubConfiguration.Owner}/{_gitHubConfiguration.Repository}/contents/{filePath}?ref={branchName}");
                if (!fileResponse.IsSuccessStatusCode)
                {
                    return new RepositoryResponse { StatusCode = (int)fileResponse.StatusCode, Message = $"Failed to fetch file {filePath}." };
                }

                var fileData = await fileResponse.Content.ReadFromJsonAsync<GitHubFileResponse>();
                if (fileData == null || string.IsNullOrEmpty(fileData.Content))
                {
                    return new RepositoryResponse { StatusCode = 400, Message = $"Invalid response for file {filePath}." };
                }

                // Decode the file content
                var decodedContent = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(fileData.Content));

                // Apply changes to the file content
                var lines = decodedContent.Split('\n').ToList();
                foreach (var change in fileChange.Value)
                {
                    //if (change.Line <= 0 || change.Line > lines.Count)
                    if (change.Line > lines.Count)
                    {
                        return new RepositoryResponse { StatusCode = 400, Message = $"Invalid line number {change.Line} for file {filePath}." };
                    }
                    //lines[change.Line - 1] = change.Change;
                    lines[0] = change.Change;
                }

                //var updatedContent = string.Join("\n", lines);
                var updatedContent = fileChange.Value.Last().Change;
                
                // Create a blob for the updated file
                var blobPayload = new { content = updatedContent, encoding = "utf-8" };
                var blobResponse = await httpClient.PostAsJsonAsync($"https://api.github.com/repos/{_gitHubConfiguration.Owner}/{_gitHubConfiguration.Repository}/git/blobs", blobPayload);
                if (!blobResponse.IsSuccessStatusCode)
                {
                    return new RepositoryResponse { StatusCode = (int)blobResponse.StatusCode, Message = $"Failed to create blob for file {filePath}." };
                }

                var blobData = await blobResponse.Content.ReadFromJsonAsync<GitHubBranchObject>();
                if (blobData == null)
                {
                    return new RepositoryResponse { StatusCode = 400, Message = "Invalid response from GitHub API." };
                }

                newTreeItems.Add(new
                {
                    path = filePath,
                    mode = "100644",
                    type = "blob",
                    sha = blobData.Sha
                });
            }

            // Create a new tree
            var treePayload = new
            {
                base_tree = commitData.Tree.Sha,
                tree = newTreeItems
            };

            var treeResponse = await httpClient.PostAsJsonAsync($"https://api.github.com/repos/{_gitHubConfiguration.Owner}/{_gitHubConfiguration.Repository}/git/trees", treePayload);
            if (!treeResponse.IsSuccessStatusCode)
            {
                return new RepositoryResponse { StatusCode = (int)treeResponse.StatusCode, Message = "Failed to create the new tree." };
            }

            var treeData = await treeResponse.Content.ReadFromJsonAsync<GitHubBranchObject>();
            if (treeData == null)
            {
                return new RepositoryResponse { StatusCode = 400, Message = "Invalid response from GitHub API." };
            }

            // Create a new commit
            var commitPayload = new
            {
                message = commitMessage,
                tree = treeData.Sha,
                parents = new[] { branchData.Object.Sha }
            };

            var newCommitResponse = await httpClient.PostAsJsonAsync($"https://api.github.com/repos/{_gitHubConfiguration.Owner}/{_gitHubConfiguration.Repository}/git/commits", commitPayload);
            if (!newCommitResponse.IsSuccessStatusCode)
            {
                return new RepositoryResponse { StatusCode = (int)newCommitResponse.StatusCode, Message = "Failed to create the new commit." };
            }

            var newCommitData = await newCommitResponse.Content.ReadFromJsonAsync<GitHubCommitResponse>();
            if (newCommitData == null)
            {
                return new RepositoryResponse { StatusCode = 400, Message = "Invalid response from GitHub API." };
            }

            // Update the branch reference to point to the new commit
            var updateBranchPayload = new { sha = newCommitData.Sha };
            var updateBranchResponse = await httpClient.PatchAsJsonAsync($"https://api.github.com/repos/{_gitHubConfiguration.Owner}/{_gitHubConfiguration.Repository}/git/refs/heads/{branchName}", updateBranchPayload);
            if (!updateBranchResponse.IsSuccessStatusCode)
            {
                return new RepositoryResponse { StatusCode = (int)updateBranchResponse.StatusCode, Message = "Failed to update the branch reference." };
            }

            return new RepositoryResponse { StatusCode = 0, Message = "Changes committed successfully." };
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
