using Web.Services.DTO_s;

namespace Web.Services;

public interface IMessageService
{
    public Task<IEnumerable<DisplayMessageDto>> GetMessages();
    public Task<IEnumerable<DisplayMessageDto>> GetAuthorMessages(string username);

    // public Task<DisplayMessageDto> CreateMessages(CreateClassroomDto dto);
}

public class MessageService(IMessageRepository messageRepository) : IMessageService
{
    public Task<IEnumerable<DisplayMessageDto>> GetMessages()
    {
        return messageRepository.GetMessages();
    }
    
    public Task<IEnumerable<DisplayMessageDto>> GetAuthorMessages(string username)
    {
        return messageRepository.GetAuthorMessages(username);
    }
    
    
    //TODO: Implement future features for creating messages
}