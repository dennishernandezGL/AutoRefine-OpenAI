using Microsoft.AspNetCore.Mvc;
using Services.Repositories;

namespace Services.Services;

[Route("api/[controller]")]
[ApiController]
public class RepositoryConnections : ControllerBase
{
    private readonly IConfiguration _configuration;

    public RepositoryConnections(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("create-branch")]
    public async Task<IActionResult> CreateBranch([FromBody] string NewBranchName)
    {
        try
        {
            var branchResponse = await ARepository.Create(Repositories.Repositories.GitHub, _configuration).CreateBranch(NewBranchName);
            if (branchResponse.StatusCode != 0)
            {
                return BadRequest(branchResponse.Message);
            }

            return Ok("Branch created successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("create-pull-request")]
    public async Task<IActionResult> CreatePullRequest([FromBody] CreatePullRequest request)
    {
        try
        {
            var branchResponse = await ARepository.Create(Repositories.Repositories.GitHub, _configuration).CreatePullRequest(request.BranchName, request.BaseBranch, request.Body);
            if (branchResponse.StatusCode != 0)
            {
                return BadRequest(branchResponse.Message);
            }

            return Ok("Pull request created successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("create-commit")]
    public async Task<IActionResult> CreateCommit([FromBody] CreateCommitRequest request)
    {
        try
        {

            var commitResponse = await ARepository.Create(Repositories.Repositories.GitHub, _configuration)
                .CommitChanges(request.FileChanges.ToDictionary(
                    change => change.FileName,
                    change => new List<string> { change.Current, change.New }),
                    request.CommitDescription,
                    request.BranchName);
            if (commitResponse.StatusCode != 0)
            {
                return BadRequest(commitResponse.Message);
            }

            return Ok("Commit created successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("checkout-branch")]
    public async Task<IActionResult> CheckoutBranch([FromBody] string branchName)
    {
        try
        {
            var checkoutResponse = await ARepository.Create(Repositories.Repositories.GitHub, _configuration).CheckoutBranch(branchName);
            if (string.IsNullOrWhiteSpace(checkoutResponse))
            {
                return BadRequest("Failed to checkout branch.");
            }

            return Ok("Branch checked out successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

public class CreateCommitRequest
    {
        public required List<FileChange> FileChanges { get; set; }
        public required string CommitDescription { get; set; }
        public required string BranchName { get; set; }
    }

public class FileChange
{
    public required string FileName { get; set; }
    public required string Current { get; set; }
    public required string New { get; set; }
}
}

public class CreatePullRequest
{
    public required string BranchName { get; set; }
    public required string BaseBranch { get; set; }
    public required string Body { get; set; }
}