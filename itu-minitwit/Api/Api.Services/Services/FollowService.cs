using Api.Services.RepositoryInterfaces;

namespace Api.Services.Services;

public interface IFollowService
{
    public Task FollowUser(string username, string follow);
    public Task UnfollowUser(string username, string unfollow);
    public IEnumerable<string> GetFollows(string username, int no);
    public Task<bool> DoesFollow(string username, string potentialFollow);
}

public class FollowService(IFollowRepository followRepository) : IFollowService
{
    // [LogMethodParameters]
    // [LogReturnValue]
    public Task FollowUser(string username, string follow)
    {
        return followRepository.Follow(username, follow);
    }

    // [LogMethodParameters]
    // [LogReturnValue]
    public Task UnfollowUser(string username, string unfollow)
    {
        return followRepository.Unfollow(username, unfollow);
    }

    // [LogMethodParameters]
    // [LogReturnValue]
    public IEnumerable<string> GetFollows(string username, int no)
    {
        return followRepository.GetFollows(username, no).Result;
    }

    public Task<bool> DoesFollow(string username, string potentialFollow)
    {
        return followRepository.DoesFollow(username, potentialFollow);
    }
}