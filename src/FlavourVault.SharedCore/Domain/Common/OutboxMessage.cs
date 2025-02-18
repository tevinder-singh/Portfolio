using FlavourVault.SharedCore.Constants;

namespace FlavourVault.SharedCore.Domain.Common;
public sealed class OutboxMessage : AggregateRoot
{
    public OutboxMessage(string eventType, string recordType, string payload)
    {
        EventType = eventType;
        Payload = payload;
        RecordType = recordType;
        Status = OutboxMessageStatus.Pending;        
    }

    public Guid? RecordId { get; set; }
    public Guid? CorrelationId { get; set; }
    public string RecordType { get; private set; }
    public string EventType { get; private set; }
    public string? TopicOrQueueName { get; set; }
    public string Payload { get; private set; }
    public DateTime EventDate { get; private set; } = DateTime.UtcNow;            
    public DateTime? ProcessedAt { get; private set; }
    public string Status { get; private set; }
    public string? ErrorMessage { get; private set; }
    public int? RetryCount { get; private set; }
    public DateTime? NextRetryDate { get; private set; }
    public void MarkAsProcessed()
    {
        Status = OutboxMessageStatus.Processed;
        ProcessedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string errorMessage)
    {   
        ProcessedAt = DateTime.UtcNow;
        Status = OutboxMessageStatus.Failed;       
        ErrorMessage = errorMessage;
    }

    public void MarkForRetryOrFail(string errorMessage, bool circuitBreaker)
    {
        RetryCount = RetryCount.GetValueOrDefault() + 1;
        if (RetryCount.HasValue && RetryCount.Value > 3)
        {
            ProcessedAt = DateTime.UtcNow;
            Status = OutboxMessageStatus.Failed;
        }
        else
        {
            NextRetryDate = DateTime.UtcNow.AddSeconds(Math.Pow((circuitBreaker ? 5 : 1), RetryCount.Value));
        }
        ErrorMessage = errorMessage;
    }
}