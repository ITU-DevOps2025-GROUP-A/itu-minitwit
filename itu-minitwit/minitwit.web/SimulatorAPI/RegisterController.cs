using itu_minitwit.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace itu_minitwit.SimulatorAPI;

[Route("[Controller]")]
[ApiController]
public class RegisterController(MiniTwitDbContext db, IPasswordHasher<User> passwordHasher, LatestService latestService)
    : ControllerBase
{
  
    [HttpPost]
    public async Task<ActionResult> Register([FromBody] RegisterRequest request, [FromQuery] int? latest)
    {
        await latestService.UpdateLatest(latest);

        if (string.IsNullOrWhiteSpace(request.Username))
        {
            return new JsonResult(new { status = 400, error_msg = "You have to enter a username" })
            {
                StatusCode = 400
            };
        }

        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
        {
            return new JsonResult(new { status = 400, error_msg = "You have to enter a valid email address" })
            {
                StatusCode = 400
            };
        }

        if (string.IsNullOrWhiteSpace(request.pwd))
        {
            return new JsonResult(new { status = 400, error_msg = "You have to enter a password" })
            {
                StatusCode = 400
            };
        }

        User user = new User
        {
            Username = request.Username,
            Email = request.Email,
        };
        user.PwHash = passwordHasher.HashPassword(user, request.pwd);

        try
        {
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            return new JsonResult(new { status = 400, error_msg = "The username is already taken" })
            {
                StatusCode = 400
            };
        }

        return Ok("");
    }
}

public class RegisterRequest
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string pwd { get; set; }
}
