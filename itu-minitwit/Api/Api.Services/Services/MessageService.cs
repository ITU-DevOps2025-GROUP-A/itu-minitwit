using Api.Services.Dto_s.MessageDTO_s;
using Api.Services.RepositoryInterfaces;

namespace Api.Services.Services;

public interface IMessageService
{
    public Task<List<DisplayMessageDTO>> ReadMessages();
    public Task<List<DisplayMessageDTO>> ReadFilteredMessages(string username, int pagesize);

    public Task<bool> PostMessage(string username, string content);
}

public class MessageService(IMessageRepository repository) : IMessageService
{
    public Task<List<DisplayMessageDTO>> ReadMessages()
    {
        return repository.ReadMessages();
    }

    public Task<List<DisplayMessageDTO>> ReadFilteredMessages(string username, int pagesize)
    {
        return repository.ReadFilteredMessages(username, pagesize);
    }

    public Task<bool> PostMessage(string username, string content)
    {
        return repository.PostMessage(username, content);
    }
}
