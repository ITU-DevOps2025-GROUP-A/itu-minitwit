using Api.Services;
using Api.Services.CustomExceptions;
using Api.Services.Dto_s;
using Api.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[Controller]")]
[ApiController]
public class RegisterController(
    IUserService userService,
    ILatestService latestService,
    ILogger<RegisterController> logger)
    : ControllerBase
{
    [LogMethodParameters]
    [LogReturnValueAsync]
    [HttpPost]
    public async Task<ActionResult> Register([FromBody] CreateUserDTO request, [FromQuery] int? latest)
    {
        try
        {
            logger.LogInformation($"Updating latest: {latest?.ToString() ?? "null"}");
            await latestService.UpdateLatest(latest);

            if (string.IsNullOrWhiteSpace(request.Username))
            {
                logger.LogError($"Invalid username: \"{request.Username}\"");
                return BadRequest(new {error_msg = "You have to enter a username"});
            }

            if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
            {
                logger.LogError($"Invalid email: \"{request.Email}\"");
                return BadRequest(new {error_msg = "You have to enter a valid email address"});
            }

            if (string.IsNullOrWhiteSpace(request.Pwd))
            {
                logger.LogError($"Invalid password: \"{request.Pwd}\"");
                return BadRequest(new {error_msg  = "You have to enter a password"});
            }

            try
            {
                await userService.Register(new CreateUserDTO()
                    { Username = request.Username, Email = request.Email, Pwd = request.Pwd });
            }
            catch (UserAlreadyExists e)
            {
                logger.LogError(e, $"User \"{request.Username}\" is already registered");
                return BadRequest(new {error_msg = "The username is already taken"});
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured, that we did have not accounted for");
            return StatusCode(500, "An error occured, that we did not for see");
        }

        return NoContent();
    }
}