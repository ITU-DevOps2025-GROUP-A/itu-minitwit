using Api.Services;
using Api.Services.CustomExceptions;
using Api.Services.Dto_s;
using Api.Services.LogDecorator;
using Api.Services.Logging;
using Api.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[Controller]")]
[ApiController]
public class RegisterController(
    IUserService userService,
    ILatestService latestService,
    ILogger<RegisterController> logger, 
    MetricsConfig metrics)
    : ControllerBase
{
    [LogTime]
    [LogMethodParameters]
    [LogReturnValueAsync]
    [HttpPost]
    public async Task<ActionResult> Register([FromBody] CreateUserDTO request, [FromQuery] int? latest)
    {
        try
        {
            logger.LogInformation("Updating latest: {Latest}", latest?.ToString() ?? "null");
            await latestService.UpdateLatest(latest);

            if (string.IsNullOrWhiteSpace(request.Username))
            {
                logger.LogError("Invalid username: \"{Username}\"", request.Username);
                return BadRequest(new {error_msg = "You have to enter a valid username"});
            }

            if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
            {
                logger.LogError("Invalid email: \"{Email}\"", request.Email);
                return BadRequest(new {error_msg = "You have to enter a valid email address"});
            }

            if (string.IsNullOrWhiteSpace(request.Pwd))
            {
                logger.LogError("Invalid password: \"{Pwd}\"", request.Pwd);
                return BadRequest(new {error_msg  = "You have to enter a valid password"});
            }

            try
            {
                await userService.Register(new CreateUserDTO()
                    { Username = request.Username, Email = request.Email, Pwd = request.Pwd });
            }
            catch (UserAlreadyExists e)
            {
                logger.LogError("User already exists, {@Error}", e);
                return BadRequest(new {error_msg = "The username is already taken"});
            }
        }
        catch (Exception e)
        {
            logger.LogException(e, "An error occured, that we have not accounted for");
            return StatusCode(500, "An error occured, that we did not for see");
        }

        metrics.RegisterCounter.Add(1);
        return NoContent();
    }
}