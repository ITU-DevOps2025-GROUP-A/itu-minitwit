using Api.Services.LogDecorator;
using Api.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[Controller]")]
[ApiController]
public class LatestController(ILatestService latestService) : ControllerBase
{
    [LogTime]
    [LogReturnValueAsync]
    [HttpGet]
    public async Task<ActionResult> GetLatest()
    {
        var latestProcessedCommandId = await latestService.GetLatest();

        return Ok(new { latest = latestProcessedCommandId });
    }
}