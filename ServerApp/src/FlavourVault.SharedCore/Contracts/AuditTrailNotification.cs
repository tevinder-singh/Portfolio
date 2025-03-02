using MediatR;

namespace FlavourVault.SharedCore.Contracts;
public sealed class AuditTrailNotification : INotification
{
    public AuditTrailNotification(string userName, string action, string actionKey, string area, string category)
    {
        UserName = userName;
        Action = action;
        ActionKey = actionKey;
        Area = area;
        Category = category;
    }

    public string UserName { get; private set; }

    public string Action { get; private set; }

    public string ActionKey { get; private set; }

    public string Area { get; private set; }

    public string Category { get; private set; }

    public DateTime CreatedOn { get; private set; } = DateTime.UtcNow;

    public string? RecordId { get; set; }

    public ICollection<AuditTrailChangeValue> ChangedValues { get; private set; } = [];
}