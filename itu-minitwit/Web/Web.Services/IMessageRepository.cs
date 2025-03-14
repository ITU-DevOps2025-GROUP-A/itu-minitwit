using Web.Services.DTO_s;

namespace Web.Services;

public interface IMessageRepository
{
    public Task<IEnumerable<DisplayMessageDto>> GetMessages();
}