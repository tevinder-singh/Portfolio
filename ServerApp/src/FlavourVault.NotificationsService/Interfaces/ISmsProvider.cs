using FlavourVault.Notification.Contracts;
using FlavourVault.Results;

namespace FlavourVault.NotificationsService.Interfaces;
internal interface ISmsProvider
{
    Task<Result> SendAsync(SmsNotification sms, CancellationToken cancellationToken);
}
