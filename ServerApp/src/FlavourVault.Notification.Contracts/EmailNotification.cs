using FlavourVault.SharedCore.Domain.Common;
using FlavourVault.SharedCore.Domain.Enums;
using System.Net.Mail;

namespace FlavourVault.Notification.Contracts;
public sealed class EmailNotification : DomainEvent
{
    public EmailNotification(string subject, string body, MailAddress from, MailAddress to, bool isBodyHtml)
    {
        EventType = "Email";
        Priority = EventPriority.High;
        TopicOrQueueName = "Notification";

        Subject = subject;
        Body = body;
        IsBodyHtml = isBodyHtml;
        From = from;
        To.Add(to);
    }

    public string Subject { get; private set; }
    public string Body { get; private set; }
    public bool IsBodyHtml { get; private set; }
    public MailAddress From { get; private set; }
    public ICollection<MailAddress> To { get; private set; } = [];
}