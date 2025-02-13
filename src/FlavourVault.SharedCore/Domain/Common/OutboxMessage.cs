namespace FlavourVault.SharedCore.Domain.Common;
public sealed class OutboxMessage : AggregateRoot
{
    public OutboxMessage(string eventType, string recordType, string payload)
    {
        EventType = eventType;
        Payload = payload;
        RecordType = recordType;
        Status = "PENDING";        
    }

    public Guid? RecordId { get; set; }
    public string RecordType { get; set; }
    public string EventType { get; private set; }
    public string? TopicOrQueueName { get; set; }
    public string Payload { get; private set; }
    public DateTime EventDate { get; private set; } = DateTime.UtcNow;            
    public DateTime? ProcessedAt { get; private set; }
    public string Status { get; private set; }
    public string? ErrorMessage { get; private set; }

    public void MarkAsProcessed()
    {
        Status = "PROCESSED";
        ProcessedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string errorMessage)
    {
        Status = "FAILED";
        ErrorMessage = errorMessage;
        ProcessedAt = DateTime.UtcNow;
    }
}
