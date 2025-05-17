using System;

namespace Services.Repositories.GitHub;

public class GitHubCommitResponse
{
    public string Sha { get; set; }
    public GitHubBranchObject Tree { get; set; }
}
