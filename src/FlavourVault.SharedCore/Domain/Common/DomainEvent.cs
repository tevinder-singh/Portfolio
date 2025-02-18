using MediatR;
using System.Text.Json.Serialization;

namespace FlavourVault.SharedCore.Domain.Common;
public abstract class DomainEvent : INotification
{
    [JsonIgnore]
    public bool IsPublished { get; set; }

    public DateTimeOffset OccurredOn { get; } = DateTime.UtcNow;
}