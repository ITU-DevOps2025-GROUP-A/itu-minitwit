namespace Api.Services.CustomExceptions;

/// <summary>
/// Exception to be thrown if some User already exist in the database
/// </summary>
public class UserAlreadyExists : Exception
{
    public UserAlreadyExists()
    {
        
    }

    public UserAlreadyExists(string message):base(message)
    {
        
    }

    public UserAlreadyExists(string message, Exception inner):base(message, inner)
    {
        
    }
}