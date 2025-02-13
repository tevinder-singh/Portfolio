using AutoMapper;
using FlavourVault.SharedCore.Contracts;
using FlavourVault.SharedCore.Domain.Common;

namespace FlavourVault.Audit.Domain.AuditTrail;
internal sealed class AuditTrail: AggregateRoot
{
    public AuditTrail() { }
    public AuditTrail(string userName, string action, string actionKey, string area, string category)
    {
        UserName = userName;
        Action = action;
        ActionKey = actionKey;
        Area = area;
        Category = category;
    }

    public string UserName { get; private set; } = null!;

    public string Action { get; private set; } = null!;

    public string ActionKey { get; private set; } = null!;

    public string Area { get; private set; } = null!;

    public string Category { get; private set; } = null!;

    public DateTime CreatedOn { get; private set; } = DateTime.UtcNow;

    public string? RecordId { get; set; }

    public ICollection<AuditTrailChangeValue>? ChangedValues { get; private set; } = [];
}

internal sealed class AuditTrailNotificationProfile : Profile
{
    public AuditTrailNotificationProfile()
    {
        CreateMap<AuditTrailNotification, AuditTrail>();
    }
}