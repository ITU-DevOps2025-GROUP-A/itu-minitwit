using Web.Services.DTO_s;
using Web.Services.Repositories;

namespace Web.Services;

public interface IFollowService
{
    public Task<bool> DoesFollow(FollowDto dto);
    public Task Follow(FollowDto dto);
    public Task UnFollow(FollowDto dto);
}

public class FollowService(IFollowRepository followRepository) : IFollowService
{
    public Task<bool> DoesFollow(FollowDto dto)
    {
        return followRepository.DoesFollow(dto);
    }

    public Task Follow(FollowDto dto)
    {
        return followRepository.Follow(dto);
    }

    public Task UnFollow(FollowDto dto)
    {
        return followRepository.UnFollow(dto);
    }
}