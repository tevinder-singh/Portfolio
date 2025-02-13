using FlavourVault.SharedCore.Contracts;
using MediatR;

namespace FlavourVault.Audit.Integrations;

internal sealed class AuditTrailNotificationHandler : INotificationHandler<AuditTrailNotification>
{
    public async Task Handle(AuditTrailNotification notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
