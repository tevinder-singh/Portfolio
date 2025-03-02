using FlavourVault.Notification.Contracts;
using FlavourVault.Results;

namespace FlavourVault.NotificationsService.Interfaces;
internal interface IEmailProvider
{
    Task<Result> SendAsync(EmailNotification email, CancellationToken cancellationToken);
}
