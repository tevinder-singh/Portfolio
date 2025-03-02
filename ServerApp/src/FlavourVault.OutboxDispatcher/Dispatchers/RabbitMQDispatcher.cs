using FlavourVault.OutboxDispatcher.Interfaces;
using FlavourVault.Results;
using FlavourVault.SharedCore.Constants;
using FlavourVault.SharedCore.Domain.Common;
using FlavourVault.SharedCore.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Client;
using System.Text;

namespace FlavourVault.OutboxDispatcher.Dispatchers;
public sealed class RabbitMQDispatcher : IMessageBusDispatcher, IDisposable
{
    private readonly ResiliencePipeline _pipeline;    
    private readonly RabbitMQOptions _rabbitMQOptions;
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly List<string> _declaredQueues = [];

    public RabbitMQDispatcher(IOptions<RabbitMQOptions> rabbitMQOptions, IOptions<ResilienceOptions> resilienceOptions, ILogger<RabbitMQDispatcher> logger)
    {
        _pipeline = resilienceOptions.Value.GetResiliencePipeline(logger);
        _rabbitMQOptions = rabbitMQOptions.Value;
    }

    private async Task InitialiseConnection(CancellationToken cancellationToken)
    {
        if (_connection != null && _channel != null)
            return;

        var factory = new ConnectionFactory();
        factory.Uri = new Uri(_rabbitMQOptions.ConnectionString);
        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await _channel.ExchangeDeclareAsync(
            exchange: _rabbitMQOptions.Exchange, 
            type: ExchangeType.Direct,
            durable: true,
            cancellationToken: cancellationToken);

        await _channel.ExchangeDeclareAsync(
            exchange: _rabbitMQOptions.DeadLetterExchange,
            type: ExchangeType.Direct,
            durable: true,
            cancellationToken: cancellationToken);        
    }

    private async Task DeclareQueue(string queueName, CancellationToken cancellationToken)
    {
        if (_declaredQueues.Contains(queueName.ToUpperInvariant()) || _channel == null)
            return;

        var retryQueueName = $"retry-{queueName}";

        //declare dead letter queue with retry setting
        await _channel.QueueDeclareAsync(
            queue: retryQueueName,
            durable: true,            
            arguments: new Dictionary<string, object?>
            {
                { "x-dead-letter-exchange", ""},
                { "x-dead-letter-routing-key", queueName},
                { "x-message-ttl", _rabbitMQOptions.DeadLetterRetryWaitTime }
            },
            cancellationToken: cancellationToken);

        await _channel.QueueBindAsync(
            queue: retryQueueName,
            exchange: _rabbitMQOptions.DeadLetterExchange,
            routingKey: retryQueueName,
            cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: queueName, 
            durable: true, 
            exclusive: false, 
            autoDelete: false,
            arguments: new Dictionary<string, object?>
            {
                { "x-dead-letter-exchange", _rabbitMQOptions.DeadLetterExchange},
                { "x-dead-letter-routing-key", retryQueueName}
            },
            cancellationToken: cancellationToken);

        await _channel.QueueBindAsync(
            queue: queueName,
            exchange: _rabbitMQOptions.Exchange,
            routingKey: queueName,
            cancellationToken: cancellationToken);

        _declaredQueues.Add(queueName.ToUpperInvariant());
    }

    public async Task<Result> DispatchAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken)
    {
        if (outboxMessage.Payload.IsEmpty())
        {
            outboxMessage.MarkAsFailed("Message paylod is empty");
            return Result.Failure("Payload", "Invalid payload");
        }

        await InitialiseConnection(cancellationToken);
        await DeclareQueue(outboxMessage.TopicOrQueueName!, cancellationToken);

        if (_channel == null)
            return Result.Failure("ChannelIssue", "Unable to create channel");

        BasicProperties properties = new BasicProperties
        {
            MessageId = outboxMessage.Id.ToString(),
            CorrelationId = outboxMessage.CorrelationId.ToString(),
            Persistent = true,
            Type = outboxMessage.EventType,
            Headers = new Dictionary<string, object?>
            {
                { OutboxMessageProprties.EventType, outboxMessage.EventType },
                { OutboxMessageProprties.RecordType, outboxMessage.RecordType },
                { OutboxMessageProprties.RecordId, outboxMessage.RecordId.GetStringValueOrEmpty() }
            }
        };

        await _pipeline.ExecuteAsync(async token =>
        {
            await _channel.BasicPublishAsync(
                exchange: _rabbitMQOptions.Exchange,
                routingKey: outboxMessage.TopicOrQueueName!,
                mandatory: true,
                basicProperties: properties,
                body: Encoding.UTF8.GetBytes(outboxMessage.Payload),
                cancellationToken: token);
        }, cancellationToken);

        return Result.Success();
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}