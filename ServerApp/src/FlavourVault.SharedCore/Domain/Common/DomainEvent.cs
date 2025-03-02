using FlavourVault.SharedCore.Domain.Enums;
using MediatR;
using System.Text.Json.Serialization;

namespace FlavourVault.SharedCore.Domain.Common;
public abstract class DomainEvent : INotification
{
    [JsonIgnore]
    public bool IsPublished { get; set; }

    [JsonIgnore]
    public EventPriority Priority { get; set; } = EventPriority.Medium;

    [JsonIgnore]
    public string? EventType { get; set; }

    [JsonIgnore]
    public string? TopicOrQueueName { get; set; }

    [JsonIgnore]
    public Guid? CorrelationId { get; set; }
}