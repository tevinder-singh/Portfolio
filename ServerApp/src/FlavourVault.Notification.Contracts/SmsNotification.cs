using FlavourVault.SharedCore.Domain.Common;
using FlavourVault.SharedCore.Domain.Enums;

namespace FlavourVault.Notification.Contracts;
public sealed class SmsNotification : DomainEvent
{
    public SmsNotification(string recipientPhoneNumber, string message)
    {
        EventType = "SMS";
        Priority = EventPriority.High;
        TopicOrQueueName = "Notification";

        RecipientPhoneNumber = recipientPhoneNumber;
        Message = message;
    }

    public string RecipientPhoneNumber { get; private set; }
    public string Message { get; private set; }    
}