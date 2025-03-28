using Api.Services.Dto_s.MessageDTO_s;
using Microsoft.AspNetCore.Mvc;
using Api.Services;
using Api.Services.Exceptions;
using Api.Services.LogDecorator;
using Api.Services.Logging;
using Api.Services.Services;

namespace Api.Controllers;

[Route("api/")]
public class MessageController(IMessageService db, ILatestService latestService, ILogger<MessageController> logger, MetricsConfig metrics) : Controller
{

    [LogTime]
    [LogMethodParameters]
    [IgnoreAntiforgeryToken]
    [HttpGet("msgs")]
    public async Task<IActionResult> GetMessages([FromQuery] int? latest, [FromQuery] int no = 100)
    {
        try
        {
            logger.LogInformation("Updating latest: {Latest}", latest?.ToString() ?? "null");
            await latestService.UpdateLatest(latest);
            var messages = await db.ReadMessages(no);
            if (messages.Count == 0)
            {
                logger.LogInformation("Didn't find any messages");
                return NoContent();
            }
            logger.LogInformation("Found {Message_count} messages, {@First_message}, {@Last_message}"
                ,messages.Count, messages.First(), messages.Last());
            return Ok(messages);
        }
        catch (Exception e)
        {
            logger.LogException(e, "An error occured, that we have not accounted for");
            return StatusCode(500, "An error occured, that we did not for see, {e}");
        }
    }

    [LogTime]
    [LogMethodParameters]
    [IgnoreAntiforgeryToken]
    [HttpGet("msgs/{username}")]
    public async Task<IActionResult> GetFilteredMessages(string username, [FromQuery] int? latest, [FromQuery] int no = 100)
    {
        try
        {
            logger.LogInformation("Updating latest: {Latest}", latest?.ToString() ?? "null");
            await latestService.UpdateLatest(latest);
            
            var filteredMessages = await db.ReadFilteredMessages(username, no);
            if (filteredMessages.Count == 0)
            {
                logger.LogInformation("Didn't find any messages");
                return NoContent();
            }
            logger.LogInformation("Found {Message_count} messages, {@First_message}, {@Last_message}"
                ,filteredMessages.Count, filteredMessages.First(), filteredMessages.Last());
            return Ok(filteredMessages);
        }
        catch (UserDoesntExistException e)
        {
            logger.LogException(e);
            return NotFound(new { message = e.Message });
        }
        catch (Exception e)
        {
            logger.LogException(e, "An error occured, that we have not accounted for");
            return StatusCode(500, "An error occured, that we did not for see");
        }
    }

    [LogTime]
    [LogMethodParameters]
    [LogReturnValueAsync]
    [IgnoreAntiforgeryToken]
    [HttpPost("msgs/{username}")]
    public async Task<IActionResult> PostMessage(string username, [FromBody] CreateMessageDTO messageDto, [FromQuery] int? latest)
    {
        try
        {
            logger.LogInformation("Updating latest: {Latest}", latest?.ToString() ?? "null");
            await latestService.UpdateLatest(latest);
            
            await db.PostMessage(username, messageDto.Content);
            metrics.MessagesCounter.Add(1);
            return NoContent();
        }
        catch (UserDoesntExistException e)
        {
            logger.LogException(e);
            return NotFound(new { message = e.Message });
        }
        catch (Exception e)
        {
            logger.LogException(e, "An error occured, that we have not accounted for");
            return StatusCode(500, "An error occured, that we did not for see");
        }
    }

    [LogTime]
    [LogMethodParameters]
    [LogReturnValueAsync]
    [HttpGet("msgs/fllws/{username}")]
    public async Task<IActionResult> GetFilteredMessagesForUserAndFollows(string username, [FromQuery] int no = 100)
    {
        try
        {
            var messages = await db.ReadFilteredMessagesFromUserAndFollows(username, no);
            if (messages.Count == 0)
            {
                logger.LogInformation("Didn't find any messages");
                return NoContent();
            }
            logger.LogInformation("Found {Message_count} messages, {@First_message}, {@Last_message}"
                ,messages.Count, messages.First(), messages.Last());
            return Ok(messages);
        }
        catch (UserDoesntExistException e)
        {
            logger.LogException(e);
            return NotFound(new { message = e.Message });
        }
        catch (Exception e)
        {
            logger.LogException(e, "An error occured, that we have not accounted for");
            return StatusCode(500, "An error occured, that we did not for see");
        }
    }
}