using Api.Services.Dto_s;
using Api.Services.LogDecorator;
using Api.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/")]
public class UserController(IUserService userService) : ControllerBase
{
    [LogTime]
    [LogMethodParameters]
    [LogReturnValueAsync]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDTO dto)
    {
        return Ok(await userService.Login(dto));
    }
}