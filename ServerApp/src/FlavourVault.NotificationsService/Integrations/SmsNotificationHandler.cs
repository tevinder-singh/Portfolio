using FlavourVault.Notification.Contracts;
using FlavourVault.NotificationsService.Exceptions;
using FlavourVault.NotificationsService.Interfaces;
using MediatR;

namespace FlavourVault.NotificationsService.Integrations;

internal sealed class SmsNotificationHandler(ISmsProvider smsProvider) : INotificationHandler<SmsNotification>
{
    public async Task Handle(SmsNotification notification, CancellationToken cancellationToken)
    {
        var result = await smsProvider.SendAsync(notification, cancellationToken);
        if (result.IsSuccess)
            return;

        throw new NotificationException(result.Errors.First().Identifier, result.Errors.First().ErrorMessage);
    }
}