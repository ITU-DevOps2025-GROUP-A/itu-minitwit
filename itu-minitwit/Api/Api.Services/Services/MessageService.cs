using Api.Services.Dto_s.MessageDTO_s;
using Api.Services.Exceptions;
using Api.Services.LogDecorator;
using Api.Services.RepositoryInterfaces;

namespace Api.Services.Services;

public interface IMessageService
{
    public Task<List<DisplayMessageDTO>> ReadMessages(int pagesize);
    public Task<List<DisplayMessageDTO>> ReadFilteredMessages(string username, int pagesize);
    public Task<List<DisplayMessageDTO>> ReadFilteredMessagesFromUserAndFollows(string username, int pagesize);

    public Task<bool> PostMessage(string username, string content);
}

public class MessageService(IMessageRepository repository, IUserService userService) : IMessageService
{
    [LogTime]
    [LogMethodParameters]
    public Task<List<DisplayMessageDTO>> ReadMessages(int pagesize)
    {
        return repository.ReadMessages(pagesize);
    }
    
    [LogTime]
    [LogMethodParameters]
    public Task<List<DisplayMessageDTO>> ReadFilteredMessages(string username, int pagesize)
    {
        return repository.ReadFilteredMessages(username, pagesize);
    }

    [LogTime]
    [LogMethodParameters]
    public Task<List<DisplayMessageDTO>> ReadFilteredMessagesFromUserAndFollows(string username, int pagesize)
    {
        return repository.ReadFilteredMessagesFromUserAndFollows(username, pagesize);
    }

    [LogTime]
    [LogMethodParameters]
    [LogReturnValue]
    public async Task<bool> PostMessage(string username, string content)
    {
        try
        {
            return await repository.PostMessage(username, content);
        }
        catch (UserDoesntExistException e)
        {
            await userService.RegisterUserFromException(username, e);
            return await repository.PostMessage(username, content);
        }
        
    }
}
