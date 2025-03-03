using Microsoft.AspNetCore.Mvc;
using itu_minitwit.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Data.Entity;
using System.Linq;
using itu_minitwit.Pages;
using itu_minitwit.SimulatorAPI;
using Newtonsoft.Json;

namespace itu_minitwit.Controllers;

[Route("/")]
public class MessageController(MiniTwitDbContext db, LatestService latestService) : Controller
{
    [IgnoreAntiforgeryToken]
    [HttpPost("add_message")]
    public IActionResult AddMessage([FromForm] string text)
    {
        // Validate message text
        if (string.IsNullOrWhiteSpace(text))
        {
            return BadRequest("Message cannot be empty.");
        }

        // Get logged-in user from session
        var username = HttpContext.Session.GetString("User");
        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized("You must be logged in to post messages.");
        }

        // Find user in database
        var user = db.Users.FirstOrDefault(u => u.Username == username);
        if (user == null)
        {
            return Unauthorized("User not found.");
        }

        // Save message to the database
        var message = new Message
        {
            AuthorId = user.UserId,
            Text = text,
            Flagged = 0,
            PubDate = (int)DateTimeOffset.Now.ToUnixTimeSeconds(),
        };

        db.Messages.Add(message);
        db.SaveChanges();

        TempData["FlashMessages"] = JsonConvert.SerializeObject(new List<string> { "Your message was recorded." });

        return Redirect("/Timeline");
    }

    [IgnoreAntiforgeryToken]
    [HttpGet("msgs")]
    public async Task<IActionResult> GetMessages([FromQuery] int? latest)
    {
        await latestService.UpdateLatest(latest);
        
        var messages = db.Messages 
            .Join(db.Users,
                m => m.AuthorId,
                a => a.UserId,
                (m,a) => new {user = a.Username, text = m.Text, pub_date = m.PubDate}
                )
            .OrderByDescending(arg => arg.pub_date)
            .Take(100)
            .ToList();

        return Json(messages);
    }


    [IgnoreAntiforgeryToken]
    [HttpGet("msgs/{username}")]
    public async Task<IActionResult> GetFilteredMessages(string username)
    {
        int pageSize = 100;
        await latestService.UpdateLatest(1);
        
        var user = db.Users.FirstOrDefault(u => u.Username == username);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }
        
        var filtered_messages = db.Messages
            .Where(m => m.AuthorId == user.UserId && m.Flagged == 0)
            .OrderByDescending(m => m.PubDate)
            .Take(pageSize)
            .Select(m => new 
            {
                user = username,
                text = m.Text,
                pub_date = m.PubDate,
            })
            .ToList();
        
        
        if (filtered_messages.Count == 0)
        {
            return NoContent();

        }

        return Ok(filtered_messages);
    }



    [HttpPost("msgs/{username}")]
    public async Task<IActionResult> PostMessage(string username, [FromForm] string content)
    {
        var user = db.Users.FirstOrDefault(u => u.Username == username);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        
        var message = new Message
        {
            AuthorId = user.UserId,
            Text = content,
            Flagged = 0,
            PubDate = (int)DateTimeOffset.Now.ToUnixTimeSeconds(),
        };
        
        db.Messages.Add(message);
        await db.SaveChangesAsync();
        
        TempData["FlashMessages"] = JsonConvert.SerializeObject(new List<string> { "Your message was recorded." });

        return NoContent();
    }
    
    
}