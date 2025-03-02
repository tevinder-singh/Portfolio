using FlavourVault.OutboxDispatcher.Interfaces;
using FlavourVault.Results;
using FlavourVault.SharedCore.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
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
        _pipeline = resilienceOptions.GetResiliencePipeline(logger);
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