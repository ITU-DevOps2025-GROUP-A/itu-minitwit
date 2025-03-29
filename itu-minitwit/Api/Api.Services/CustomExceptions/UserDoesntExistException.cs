namespace Api.Services.Exceptions;

public class UserDoesntExistException : Exception
{
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