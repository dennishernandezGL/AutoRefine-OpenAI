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
}

public class CreatePullRequest
{
    public required string BranchName { get; set; }
    public required string BaseBranch { get; set; }
    public required string Body { get; set; }
}