using Azure.Messaging.ServiceBus;
using FlavourVault.OutboxDispatcher.Interfaces;
using FlavourVault.Results;
using FlavourVault.SharedCore.Domain.Common;
using FlavourVault.SharedCore.Extensions;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace FlavourVault.OutboxDispatcher.Dispatchers;
public sealed class AzureServiceBusDispatcher : IMessageBusDispatcher
{
    private readonly IAzureClientFactory<ServiceBusSender> _serviceBusSenderFactory;
    private readonly ResiliencePipeline _pipeline;
    public AzureServiceBusDispatcher(IAzureClientFactory<ServiceBusSender> serviceBusSenderFactory, IOptions<ResilienceOptions> options, ILogger<AzureServiceBusDispatcher> logger)
    {
        var resilienceOptions = options.Value;
        _serviceBusSenderFactory = serviceBusSenderFactory;
        _pipeline = GetResiliencePipeline(logger, resilienceOptions);
    }

    private ResiliencePipeline GetResiliencePipeline(ILogger<AzureServiceBusDispatcher> logger, ResilienceOptions resilienceOptions)
    {
        return new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = resilienceOptions.MaxRetryAttempts,
                Delay = TimeSpan.FromMilliseconds(resilienceOptions.DelayMilliseconds),
                BackoffType = resilienceOptions.BackoffType,
                UseJitter = resilienceOptions.UseJitter,
                ShouldHandle = new PredicateBuilder().Handle<Exception>(),
                OnRetry = (args) =>
                {
                    logger.LogError("Retrying due to: {Message}, Attempt: {AttemptNumber}", args.Outcome.Exception?.Message, args.AttemptNumber);
                    return ValueTask.CompletedTask;
                }
            })
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                FailureRatio = 0.3,
                SamplingDuration = TimeSpan.FromSeconds(30),
                BreakDuration = TimeSpan.FromSeconds(1),
                MinimumThroughput = 5,
                OnOpened = (args) =>
                {
                    logger.LogError("Circuit breaker triggered due to: {Message}", args.Outcome.Exception?.Message);
                    return ValueTask.CompletedTask;
                },
                OnClosed = (args) =>
                {
                    logger.LogInformation("Circuit breaker reset.");
                    return ValueTask.CompletedTask;
                }
            })
            .AddTimeout(TimeSpan.FromSeconds(5))
            .Build();
    }

    public async Task<Result> DispatchAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken)
    {
        if (outboxMessage.Payload.IsEmpty())
        {
            outboxMessage.MarkAsFailed("Message paylod is empty");
            return Result.Failure("Payload", "Invalid payload");
        }

        var sender = _serviceBusSenderFactory.CreateClient(outboxMessage.TopicOrQueueName);

        await _pipeline.ExecuteAsync(async token =>
        {
            await sender.SendMessageAsync(new ServiceBusMessage(outboxMessage.Payload)
            {
                MessageId = outboxMessage.Id.ToString(),
                CorrelationId = outboxMessage.CorrelationId.GetStringValueOrEmpty(),                
                ApplicationProperties =
                {
                    { "EventType", outboxMessage.EventType },
                    { "RecordType", outboxMessage.RecordType },
                    { "RecordId", outboxMessage.RecordId.GetStringValueOrEmpty() }
                }
            }, token);
        }, cancellationToken);

        return Result.Success();
    }

}