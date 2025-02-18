using FlavourVault.SharedCore.Domain.Common;

namespace FlavourVault.SharedCore.Domain.DomainEvents;
public sealed class AuditTrailNotification : DomainEvent
{
    public AuditTrailNotification(string userName, string action, string actionKey, string area, string category, Guid? recordId)
    {
        UserName = userName;
        Action = action;
        ActionKey = actionKey;
        Area = area;
        Category = category;
        RecordId = recordId;
    }

    public string UserName { get; private set; }

    public string Action { get; private set; }

    public string ActionKey { get; private set; }

    public string Area { get; private set; }

    public string Category { get; private set; }

    public Guid? RecordId { get; set; }

    public ICollection<AuditTrailChangeValue> ChangedValues { get; private set; } = [];
}

public record AuditTrailChangeValue(string Key, object? From, object? To);