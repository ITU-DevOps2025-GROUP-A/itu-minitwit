using Microsoft.Extensions.Logging;

namespace Api.Services.Logging;

public static class LoggerExtension
{
    public static void LogException(this ILogger logger, Exception e, string message = "")
    {
        ArgumentNullException.ThrowIfNull(logger);
        
        if(string.IsNullOrWhiteSpace(message))
        {
            logger.LogError("{ErrorName}, Message:\"{ErrorMessage}\", {@Error}", e.GetType().Name, e.Message, e);
            return;
        }
        
        logger.LogError("{message}, {ErrorName}, Message:\"{ErrorMessage}\", {@Error}", message, e.GetType().Name, e.Message, e);
    }

    public static void LogThrowingException(this ILogger logger, Exception e, string message = "")
    {
        ArgumentNullException.ThrowIfNull(logger);
        
        if(string.IsNullOrWhiteSpace(message))
        {
            logger.LogInformation("Throwing: {ErrorName}, Message:\"{ErrorMessage}\", {@Error}", e.GetType().Name, e.Message, e);
            return;
        }
        
        logger.LogInformation("Throwing: {ErrorName}, {message}, Message:\"{ErrorMessage}\", {@Error}", message, e.GetType().Name, e.Message, e);
    }
}