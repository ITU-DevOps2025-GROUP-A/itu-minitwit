using Microsoft.AspNetCore.Mvc;
using itu_minitwit.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using Newtonsoft.Json;

namespace itu_minitwit.Controllers;

[Route("/")]
public class MessageController(MiniTwitDbContext db) : Controller
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
}