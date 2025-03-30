
namespace Api.Services.RepositoryInterfaces;

public interface IFollowRepository
{
    public Task Follow(string username, string follow);
    
    public Task Unfollow(string username, string follow);
    
    public Task<IEnumerable<string>> GetFollows(string username, int no = 100);
    
    public Task<bool> DoesFollow(string username, string potentialFollow);
}