using Web.Services.DTO_s;

namespace Web.Services;

public interface IFollowService
{
    public Task<bool> DoesFollow(FollowDto dto);
    public Task Follow(FollowDto dto);
    public Task UnFollow(FollowDto dto);
}

public class FollowService : IFollowService
{
    public Task<bool> DoesFollow(FollowDto dto)
    {
        throw new NotImplementedException();
    }

    public Task Follow(FollowDto dto)
    {
        throw new NotImplementedException();
    }

    public Task UnFollow(FollowDto dto)
    {
        throw new NotImplementedException();
    }
}