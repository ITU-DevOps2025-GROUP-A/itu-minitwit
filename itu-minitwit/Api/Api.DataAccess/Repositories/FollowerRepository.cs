using Api.Services.Exceptions;
using Api.Services.RepositoryInterfaces;
using Api.DataAccess.Models;
using Api.Services;
using Api.Services.CustomExceptions;
using Api.Services.LogDecorator;
using Api.Services.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace Api.DataAccess.Repositories;

public class FollowRepository(MinitwitDbContext dbContext, ILogger<FollowRepository> logger) : IFollowRepository
{
    [LogTime]
    [LogMethodParameters]
    [LogReturnValueAsync]
    public async Task Follow(string username, string follow)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        var userToFollow = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == follow);

        if (user == null)
        {
            var e = new UserDoesntExistException($"User: \"{username}\" not found");
            logger.LogException(e);
            throw e;
        }
        if(userToFollow == null)
        {
            var e =new UserDoesntExistException($"User: \"{follow}\" not found");
            logger.LogException(e);
            throw e;
        }
        
        if (!await dbContext.Followers.AnyAsync(f => f.WhoId == user.UserId
                                             && f.WhomId == userToFollow.UserId))
        {
            var followRelation = new Follower
            {
                WhoId = user.UserId,
                WhomId = userToFollow.UserId
            };

            await dbContext.Followers.AddAsync(followRelation);
            await dbContext.SaveChangesAsync();
        }
    }

    [LogTime]
    [LogMethodParameters]
    [LogReturnValueAsync]
    public async Task Unfollow(string username, string unfollow)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        var userToUnfollow = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == unfollow);
        
        if (user == null)
        {
            var e = new UserDoesntExistException($"User: \"{username}\" not found");
            logger.LogException(e);
            throw e;
        }
        if(userToUnfollow == null)
        {
            var e =new UserDoesntExistException($"User: \"{unfollow}\" not found");
            logger.LogException(e);
            throw e;
        }
        
        var followRelation =
            await dbContext.Followers.FirstOrDefaultAsync(f => f.WhoId == user!.UserId
                                                    && f.WhomId == userToUnfollow!.UserId);
        
        if (followRelation != null)
        {
            dbContext.Followers.Remove(followRelation);
            await dbContext.SaveChangesAsync();
        }
    }

    [LogTime]
    [LogMethodParameters]
    [LogReturnValueAsync]
    public async Task<IEnumerable<string>> GetFollows(string username, int no = 100)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null)
        {
            var e = new UserDoesntExistException($"User: \"{username}\" not found");
            logger.LogException(e);
            throw e;
        }

        var followsList = await dbContext.Followers
            .AsNoTracking()
            .Where(f => f.WhoId == user.UserId)
            .Join(dbContext.Users,
                f => f.WhomId,
                u => u.UserId,
                (f, u) => u.Username
            )
            .Take(no)
            .ToListAsync();
        
        return followsList;
    }

    public async Task<bool> DoesFollow(string username, string potentialFollow)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        var potentialFollowUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == potentialFollow);

        if (user == null)
        {
            var e =new UserDoesntExistException($"\"{username}\" not found");
            // logger.LogError($"{e.Message} - throw: {e.GetType()}");
            throw e;
        }

        if (potentialFollowUser == null)
        {
            var e =new UserDoesntExistException($"\"{potentialFollow}\" not found");
            throw e;
        }

        return await dbContext.Followers.AnyAsync(fr => fr.WhoId == user.UserId
                                                        && fr.WhomId == potentialFollowUser.UserId);
    }
}