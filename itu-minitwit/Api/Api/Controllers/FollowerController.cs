using Api.Services;
using Api.Services.CustomExceptions;
using Api.Services.Dto_s.FollowDTO_s;
using Api.Services.Exceptions;
using Api.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/")]
[ApiController]
public class FollowerController(IFollowService followService, ILatestService latestService, ILogger<FollowerController> logger) : ControllerBase
{
    [LogMethodParameters]
    [LogReturnValueAsync]
    [HttpPost("fllws/{username}")]
    public async Task<ActionResult> FollowOrUnfollow(string username, [FromBody] FollowDTO followDto, [FromQuery] int? latest)
    {
        try
        {
            await latestService.UpdateLatest(latest);
            logger.LogInformation($"Updating latest: {latest?.ToString() ?? "null"}");
        
            if (!String.IsNullOrWhiteSpace(followDto.Follow))
            {
                if (username == followDto.Follow)
                {
                    logger.LogError("You cannot follow yourself");
                    return BadRequest("You cannot follow yourself");
                }
                
                return await Follow(username, followDto.Follow);
            }
            if (!String.IsNullOrWhiteSpace(followDto.Unfollow))
            {
                if (username == followDto.Unfollow)
                {
                    logger.LogError("You cannot unfollow yourself");
                    return BadRequest("You cannot unfollow yourself");
                }
                
                return await Unfollow(username, followDto.Unfollow);
            }
            
            logger.LogError("You must provide a user to follow or unfollow");
            return BadRequest("You must provide a user to follow or unfollow");
        } 
        catch (Exception e)
        {
            logger.LogError(e, "An error occured, that we did have not accounted for");
            return StatusCode(500, "An error occured, that we did not for see");
        }
    }

    [LogMethodParameters]
    [LogReturnValueAsync]
    private async Task<ActionResult> Follow(string username, string follow)
    {
        try
        {
            await followService.FollowUser(username, follow);
        }
        catch (UserDoesntExistException e)
        {
            logger.LogError(e, "User does not exists");
            return NotFound(e.Message);
        }

        return NoContent();
    }
    
    [LogMethodParameters]
    [LogReturnValueAsync]
    private async Task<ActionResult> Unfollow(string username, string unfollow)
    {
        try
        {
            await followService.UnfollowUser(username, unfollow);
        }
        catch (UserDoesntExistException e)
        {
            logger.LogError(e, "User does not exists");
            return NotFound(e.Message);
        }

        return NoContent();
    }

    [LogMethodParameters]
    [LogReturnValueAsync]
    [HttpGet("fllws/{username}")]
    public async Task<IActionResult> GetFollows(string username, [FromQuery] int? latest, [FromQuery] int no = 100)
    {
        try
        {
            await latestService.UpdateLatest(latest);
            logger.LogInformation($"Updating latest: {latest?.ToString() ?? "null"}");

            var follows = followService.GetFollows(username, no);
            return Ok(new { follows });
        }
        catch (UserDoesntExistException e)
        {
            logger.LogError(e, $"\"{username}\" does not exists");
            return NotFound(new { message = e.Message });
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured, that we did have not accounted for");
            return StatusCode(500, "An error occured, that we did not for see");
        }
    }

    [HttpGet("fllws/{username}/{potentialFollow}")]
    public async Task<IActionResult> DoesFollow(string username, string potentialFollow)
    {
        if (username == potentialFollow) return BadRequest("You can't follow yourself");
        return Ok (await followService.DoesFollow(username, potentialFollow));
    }

}
