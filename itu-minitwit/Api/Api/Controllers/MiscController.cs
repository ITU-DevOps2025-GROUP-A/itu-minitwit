using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class MiscController : ControllerBase
{
    [HttpGet("health")]
    public async Task<IActionResult> Health()
    {
        await Task.Delay(1);
        return Ok();
    }
}