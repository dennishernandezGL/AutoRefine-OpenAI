using Microsoft.AspNetCore.Mvc;
using Services.Helpers;

namespace Services.Services;

[Route("api/[controller]")]
[ApiController]
public class PlaywrightController : ControllerBase
{
    private readonly PlaywrightService _playwrightService;
    
    public PlaywrightController(PlaywrightService playwrightService)
    {
        _playwrightService = playwrightService;
    }
    
    [HttpGet("/playwright/browsers")]
    public async Task<IActionResult> ExecuteTests()
    {
        await _playwrightService.ExecuteTests();
        return Ok();
    }
    
    [HttpPost("/playwright/run/{branchName}")]
    public async Task<IActionResult> Run(string branchName)
    {
        await _playwrightService.RunTests(branchName);
        return Ok();
    }
}