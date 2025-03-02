using FlavourVault.SharedCore.Domain.DomainEvents;
using MediatR;

namespace FlavourVault.Audit.Integrations;

internal sealed class AuditTrailNotificationHandler : INotificationHandler<AuditTrailNotification>
{
    public async Task Handle(AuditTrailNotification notification, CancellationToken cancellationToken)
    {
        //throw new Exception("Audit Error");
    }
}
