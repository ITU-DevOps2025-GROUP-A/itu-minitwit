using Microsoft.AspNetCore.Mvc;
using itu_minitwit.Data;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using itu_minitwit.SimulatorAPI;

namespace itu_minitwit.Controllers
{
    [Route("/")]
    public class MessageController(MiniTwitDbContext db, LatestService latestService) : Controller    {
       

        // Endpoint to add a message, expects JSON input
        [IgnoreAntiforgeryToken]
        [HttpPost("add_message")]
        public IActionResult AddMessage([FromBody] AddMessageRequest request)
        {
            // Validate message text
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return BadRequest(new { status = 400, error_msg = "Message cannot be empty." });
            }

            // Get logged-in user from session
            var username = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(new { status = 401, error_msg = "You must be logged in to post messages." });
            }

            // Find user in database
            var user = db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return Unauthorized(new { status = 401, error_msg = "User not found." });
            }

            // Save message to the database
            var message = new Message
            {
                AuthorId = user.UserId,
                Text = request.Text,
                Flagged = 0,
                PubDate = (int)DateTimeOffset.Now.ToUnixTimeSeconds(),
            };

            db.Messages.Add(message);
            db.SaveChanges();

            return Ok(new { status = 200, message = "Your message was recorded." });
        }

        // Endpoint to get messages, returning JSON response
        [IgnoreAntiforgeryToken]
        [HttpGet("msgs")]
        public async Task<IActionResult> GetMessages([FromQuery] int? latest)
        {
            await latestService.UpdateLatest(latest);
            
            var messages = db.Messages 
                .Join(db.Users,
                    m => m.AuthorId,
                    a => a.UserId,
                    (m, a) => new { user = a.Username, text = m.Text, pub_date = m.PubDate }
                )
                .OrderByDescending(arg => arg.pub_date)
                .Take(100)
                .ToList();

            return Json(messages);
        }

        // Endpoint to get filtered messages for a specific user, returning JSON response
        [IgnoreAntiforgeryToken]
        [HttpGet("msgs/{username}")]
        public async Task<IActionResult> GetFilteredMessages(string username, [FromQuery] int? latest)
        {
            int pageSize = 100;
            await latestService.UpdateLatest(latest);
            
            var user = db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }
            
            var filteredMessages = db.Messages
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
            
            if (filteredMessages.Count == 0)
            {
                return NoContent();
            }

            return Ok(filteredMessages);
        }

        // Endpoint to post a message for a user, expects JSON input
        [HttpPost("msgs/{username}")]
        public async Task<IActionResult> PostMessage(string username, [FromBody] PostMessageRequest request)
        {
            var user = db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var message = new Message
            {
                AuthorId = user.UserId,
                Text = request.Content,
                Flagged = 0,
                PubDate = (int)DateTimeOffset.Now.ToUnixTimeSeconds(),
            };
            
            db.Messages.Add(message);
            await db.SaveChangesAsync();

            return Ok(new { status = 200, message = "Your message was recorded." });
        }
    }

    // Request model for adding a message
    public class AddMessageRequest
    {
        public string Text { get; set; }
    }

    // Request model for posting a message
    public class PostMessageRequest
    {
        public string Content { get; set; }
    }
}
