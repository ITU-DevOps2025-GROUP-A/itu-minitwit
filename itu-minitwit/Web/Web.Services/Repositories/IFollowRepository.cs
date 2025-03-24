using Web.Services.DTO_s;

namespace Web.Services.Repositories;

public interface IFollowRepository
{
    public Task<bool> DoesFollow(FollowDto dto);
    public Task Follow(FollowDto dto);
    public Task UnFollow(FollowDto dto);
}