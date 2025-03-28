using Api.DataAccess.Models;
using Api.Services;
using Api.Services.Dto_s.MessageDTO_s;
using Api.Services.Exceptions;
using Api.Services.LogDecorator;
using Api.Services.Logging;
using Api.Services.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Api.DataAccess.Repositories;

public class MessageRepository(MinitwitDbContext dbContext, ILogger<MessageRepository> logger) : IMessageRepository
{
    [LogTime]
    [LogMethodParameters]
    public async Task<List<DisplayMessageDTO>> ReadMessages(int pagesize)
    {
        if(!await dbContext.Messages.AnyAsync()) return Enumerable.Empty<DisplayMessageDTO>().ToList();
        
        return await dbContext.Messages
            .AsNoTracking()
            .Where(m => m.Flagged == 0)
            .OrderByDescending(m => m.PubDate)
            .Take(pagesize)
            .Select(m => new DisplayMessageDTO
            {
                Username = m.Author!.Username,
                Email = m.Author!.Email,
                Text = m.Text,
                PubDate = m.PubDate,
            })
            .ToListAsync();
    }

    [LogTime]
    [LogMethodParameters]
    public async Task<List<DisplayMessageDTO>> ReadFilteredMessages(string username, int pagesize = 100)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
        {
            var e = new UserDoesntExistException($"User: \"{username}\" not found");
            logger.LogException(e);
            throw e;
        }
        
        if(!await dbContext.Messages.AnyAsync()) return Enumerable.Empty<DisplayMessageDTO>().ToList();

        return await dbContext.Messages
            .AsNoTracking()
            .Where(m => m.AuthorId == user.UserId && m.Flagged == 0)
            .OrderByDescending(m => m.PubDate)
            .Take(pagesize)
            .Select(m => new DisplayMessageDTO
            {
                Username = m.Author!.Username,
                Email = m.Author!.Email,
                Text = m.Text,
                PubDate = m.PubDate,
            })
            .ToListAsync();
    }

    public async Task<List<DisplayMessageDTO>> ReadFilteredMessagesFromUserAndFollows(string username, int pagesize = 100)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
        {
            var e = new UserDoesntExistException($"User: \"{username}\" not found");
            logger.LogException(e);
            throw e;
        }

        if(!await dbContext.Messages.AnyAsync()) return Enumerable.Empty<DisplayMessageDTO>().ToList();
        
        return await dbContext.Messages
            .AsNoTracking()
            .Where(m => m.Flagged == 0 &&
                        (m.AuthorId == user.UserId ||
                         dbContext.Followers.Any(f => f.WhoId == user.UserId && f.WhomId == m.AuthorId)))
            .OrderByDescending(m => m.PubDate)
            .Take(pagesize)
            .Select(m => new DisplayMessageDTO
            {
                Username = m.Author!.Username,
                Email = m.Author!.Email,
                Text = m.Text,
                PubDate = m.PubDate,
            })
            .ToListAsync();
    }

    [LogTime]
    [LogMethodParameters]
    [LogReturnValue]
    public async Task<bool> PostMessage(string username, string content)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);

        if (user == null)
        {
            var e = new UserDoesntExistException($"User: \"{username}\" not found");
            logger.LogException(e);
            throw e;
        }

        var message = new Message
        {
            AuthorId = user.UserId,
            Text = content,
            Flagged = 0,
            PubDate = (int)DateTimeOffset.Now.ToUnixTimeSeconds(),
        };

        await dbContext.Messages.AddAsync(message);
        await dbContext.SaveChangesAsync();
        return true;
    }
}