namespace Api.Services.Exceptions;

public class UserDoesntExistException : Exception
{
    public bool UserExists { get; init; }
    public bool FollowExists { get; init; }

    public UserDoesntExistException()
    {
        
    }

    public UserDoesntExistException(string message):base(message)
    {
        
    }

    public UserDoesntExistException(string message, Exception inner):base(message, inner)
    {
        
    }
}