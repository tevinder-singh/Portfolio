using Microsoft.Extensions.Logging;

namespace FlavourVault.SharedCore.Extensions;
public static class LoggerExtensions
{
    private static readonly Action<ILogger, string, string, Exception> _exceptionMessage = 
        LoggerMessage.Define<string, string>(LogLevel.Error, 1, "Exception occurred: {Message}. {ExceptionId}");

    private static readonly Action<ILogger, string, Exception> _exceptionMessageWithoutId =
        LoggerMessage.Define<string>(LogLevel.Error, 1, "Exception occurred: {Message}.");

    public static void LogException(this ILogger logger, Exception exception, Guid exceptionId) => _exceptionMessage(logger, exception.Message, exceptionId.ToString(), exception);
    public static void LogException(this ILogger logger, Exception exception) => _exceptionMessageWithoutId(logger, exception.Message, exception);
}
