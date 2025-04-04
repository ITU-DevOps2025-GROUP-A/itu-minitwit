using Api.Services.Dto_s;
using Api.Services.Exceptions;
using Api.Services.LogDecorator;
using Api.Services.RepositoryInterfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Services.Services;

public interface IFollowService
{
    public Task FollowUser(string username, string follow);
    public Task UnfollowUser(string username, string unfollow);
    public IEnumerable<string> GetFollows(string username, int no);
    public Task<bool> DoesFollow(string username, string potentialFollow);
}

public class FollowService(IFollowRepository followRepository, IUserService userService) : IFollowService
{
    [LogTime]
    [LogMethodParameters]
    [LogReturnValue]
    public async Task FollowUser(string username, string follow)
    {
        try
        {
            await followRepository.Follow(username, follow);
        }
        catch (UserDoesntExistException e)
        {
            await userService.RegisterUsersFromException(username, follow, e);
            await followRepository.Follow(username, follow);
        }
    }

    [LogTime]
    [LogMethodParameters]
    [LogReturnValue]
    public async Task UnfollowUser(string username, string unfollow)
    {
        try
        {
            await followRepository.Unfollow(username , unfollow);
        }
        catch (UserDoesntExistException e)
        {
            await userService.RegisterUsersFromException(username, unfollow, e);
            await followRepository.Unfollow(username, unfollow);
        }
    }

    [LogTime]
    [LogMethodParameters]
    [LogReturnValue]
    public IEnumerable<string> GetFollows(string username, int no)
    {
        return followRepository.GetFollows(username, no).Result;
    }

    [LogTime]
    [LogMethodParameters]
    [LogReturnValue]
    public Task<bool> DoesFollow(string username, string potentialFollow)
    {
        return followRepository.DoesFollow(username, potentialFollow);
    }
}