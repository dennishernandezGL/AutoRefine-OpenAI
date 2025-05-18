using Services.Repositories.GitHub;

namespace Services.Repositories;

public abstract class ARepository
{
    public static ARepository Create(Repositories repo, IConfiguration configuration)
    {
        switch (repo)
        {
            case Repositories.GitHub:
                return new GitHubRepository(configuration);
            default:
                throw new NotImplementedException($"Repository {repo} is not implemented.");
        }
    }
    
    public abstract Task CheckoutBranch(string branchName);
    public abstract Task<RepositoryResponse> CreateBranch(string NewBranchName);
    public abstract Task<RepositoryResponse> CreatePullRequest(string BranchName, string BaseBranch, string Body);
    public abstract Task<RepositoryResponse> CommitChanges(Dictionary<string, List<string>> fileChanges, string commitMessage, string branchName);
    public abstract Task<List<Models.RepositoryFile>> GetRepositoryFilesAsync();
}
