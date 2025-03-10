using Api.Services.Dto_s.MessageDTO_s;
using Microsoft.AspNetCore.Mvc;
using Api.Services.Services;

namespace itu_minitwit.Controllers;

[Route("/")]
public class MessageController(IMessageService db, ILatestService latestService) : Controller
{

    [IgnoreAntiforgeryToken]
    [HttpGet("msgs")]
    public async Task<IActionResult> GetMessages([FromQuery] int? latest)
    {
        await latestService.UpdateLatest(latest);

        var messages = db.ReadMessages().Result;

        return Json(messages);
    }


    [IgnoreAntiforgeryToken]
    [HttpGet("msgs/{username}")]
    public async Task<IActionResult> GetFilteredMessages(string username)
    {
        await latestService.UpdateLatest(1);
        try
        {
            var filteredMessages = await db.ReadFilteredMessages(username, 100);
            if (filteredMessages.Count == 0)
            {
                return NoContent();

            }
            return Ok(filteredMessages);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
    }


    [IgnoreAntiforgeryToken]
    [HttpPost("msgs/{username}")]
    public async Task<IActionResult> PostMessage(string username, [FromBody] CreateMessageDTO messageDto)
    {
        
        try
        {
            await db.PostMessage(username, messageDto.Content);
            return NoContent();
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
        
        
    }
    
    
}