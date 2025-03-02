namespace FlavourVault.NotificationsService.Exceptions;
public class NotificationException : Exception
{
    public NotificationException()
    {
    }

    public NotificationException(string message) : base(message)
    {
    }

    public NotificationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public NotificationException(string message, string errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }

    public NotificationException(string message, string errorCode, Exception innerException) : base(message, innerException)
    {
        ErrorCode = errorCode;
    }

    public string? ErrorCode { get; set; }

}
