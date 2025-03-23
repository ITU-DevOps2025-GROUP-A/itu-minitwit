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
    ILogger<RegisterController> logger, 
    MetricsConfig metrics)
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
                return new JsonResult(new { status = 400, error_msg = "You have to enter a username" })
                {
                    StatusCode = 400
                };
            }

            if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
            {
                logger.LogError($"Invalid email: \"{request.Email}\"");
                return new JsonResult(new { status = 400, error_msg = "You have to enter a valid email address" })
                {
                    StatusCode = 400
                };
            }

            if (string.IsNullOrWhiteSpace(request.Pwd))
            {
                logger.LogError($"Invalid password: \"{request.Pwd}\"");
                return new JsonResult(new { status = 400, error_msg = "You have to enter a password" })
                {
                    StatusCode = 400
                };
            }

            try
            {
                await userService.Register(new CreateUserDTO()
                    { Username = request.Username, Email = request.Email, Pwd = request.Pwd });
            }
            catch (UserAlreadyExists e)
            {
                logger.LogError(e, $"User \"{request.Username}\" is already registered");
                return new JsonResult(new { status = 400, error_msg = "The username is already taken" })
                {
                    StatusCode = 400
                };
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured, that we did have not accounted for");
            return StatusCode(500, "An error occured, that we did not for see");
        }

        metrics.RegisterCounter.Add(1);
        return NoContent();
    }
}