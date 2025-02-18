using FlavourVault.OutboxDispatcher.Interfaces;
using FlavourVault.Results;
using FlavourVault.SharedCore.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using System.Collections.Concurrent;
using System.Text.Json;

namespace FlavourVault.OutboxDispatcher.Dispatchers;

public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private static readonly ConcurrentDictionary<string, Type?> TypeCache = new();
    private readonly IMediator _sender;    
    private readonly ResiliencePipeline _pipeline;
    public DomainEventDispatcher(IMediator sender, IOptions<ResilienceOptions> options, ILogger<DomainEventDispatcher> logger)
    {
        var resilienceOptions = options.Value;
        _sender = sender;
        _pipeline = GetResiliencePipeline(logger, resilienceOptions);
    }

    private ResiliencePipeline GetResiliencePipeline(ILogger<DomainEventDispatcher> logger, ResilienceOptions resilienceOptions)
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
        var messageType = GetOrAddMessageType(outboxMessage.EventType);
        if (messageType == null)
        {
            outboxMessage.MarkAsFailed("Invalid Message Type");
            return Result.Failure("Invalid", "Invalid message type");
        }

        var domainEvent = JsonSerializer.Deserialize(outboxMessage.Payload, messageType);
        if (domainEvent == null)
        {
            outboxMessage.MarkAsFailed($"Message paylod is empty or unable to convert to {messageType}");
            return Result.Failure("Payload", "Invalid payload");
        }
        
        await _pipeline.ExecuteAsync(async token =>
        {
            await _sender.Publish(domainEvent, token);
        }, cancellationToken);

        return Result.Success();
    }

    private static Type? GetOrAddMessageType(string typeName)
    {
        return TypeCache.GetOrAdd(typeName, GetTypeByName(typeName));
    }

    private static Type? GetTypeByName(string name)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
                .Reverse()
                .Select(assembly => assembly.GetType(name))
                .FirstOrDefault(t => t != null);
    }
}