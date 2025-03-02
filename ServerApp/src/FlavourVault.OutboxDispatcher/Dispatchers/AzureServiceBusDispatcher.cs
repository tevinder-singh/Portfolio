using Azure.Messaging.ServiceBus;
using FlavourVault.OutboxDispatcher.Interfaces;
using FlavourVault.Results;
using FlavourVault.SharedCore.Constants;
using FlavourVault.SharedCore.Domain.Common;
using FlavourVault.SharedCore.Extensions;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

namespace FlavourVault.OutboxDispatcher.Dispatchers;
public sealed class AzureServiceBusDispatcher : IMessageBusDispatcher
{
    private readonly IAzureClientFactory<ServiceBusSender> _serviceBusSenderFactory;
    private readonly ResiliencePipeline _pipeline;
    public AzureServiceBusDispatcher(IAzureClientFactory<ServiceBusSender> serviceBusSenderFactory, IOptions<ResilienceOptions> options, ILogger<AzureServiceBusDispatcher> logger)
    {
        var resilienceOptions = options.Value;
        _serviceBusSenderFactory = serviceBusSenderFactory;
        _pipeline = resilienceOptions.GetResiliencePipeline(logger);
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
                    { OutboxMessageProprties.EventType, outboxMessage.EventType },
                    { OutboxMessageProprties.RecordType, outboxMessage.RecordType },
                    { OutboxMessageProprties.RecordId, outboxMessage.RecordId.GetStringValueOrEmpty() }
                }
            }, token);
        }, cancellationToken);

        return Result.Success();
    }

}