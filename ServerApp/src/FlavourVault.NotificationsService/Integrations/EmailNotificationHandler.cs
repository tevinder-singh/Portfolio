using FlavourVault.Notification.Contracts;
using FlavourVault.NotificationsService.Exceptions;
using FlavourVault.NotificationsService.Interfaces;
using MediatR;

namespace FlavourVault.NotificationsService.Integrations;

internal sealed class EmailNotificationHandler(IEmailProvider emailProvider) : INotificationHandler<EmailNotification>
{
    public async Task Handle(EmailNotification notification, CancellationToken cancellationToken)
    {
        var result = await emailProvider.SendAsync(notification, cancellationToken);
        if (result.IsSuccess)
            return;

        throw new NotificationException(result.Errors.First().Identifier, result.Errors.First().ErrorMessage);
    }
}
