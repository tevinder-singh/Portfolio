using FlavourVault.Notification.Contracts;
using FlavourVault.NotificationsService.Configurations;
using FlavourVault.NotificationsService.Interfaces;
using FlavourVault.SharedCore.Constants;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace FlavourVault.NotificationsService.Consumers;
internal sealed class RabbitMQConsumer : IMessageConsumer
{
    private readonly ILogger<RabbitMQConsumer> _logger;
    private IConnection? _connection = null!;
    private IChannel? _channel = null!;
    private readonly RabbitMQConsumerOptions _rabbitMQOptions;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly Dictionary<string, Type> _typeCache = new()
    {
        { "SMS", typeof(SmsNotification) },
        { "Email", typeof(SmsNotification) }
    };

    private const int _queueExistCheckWaitBetweenIterationMilliSecond = 100_00;

    public RabbitMQConsumer(ILogger<RabbitMQConsumer> logger, IServiceScopeFactory serviceScopeFactory, IOptions<RabbitMQConsumerOptions> rabbitMQOptions)
    {
        _logger = logger;        
        _serviceScopeFactory = serviceScopeFactory;
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
    }

    private async Task DeclareQueue(CancellationToken cancellationToken)
    {
        try
        {
            await InitialiseConnection(cancellationToken);
            if (_channel == null)
                return;

            _ = await _channel.QueueDeclarePassiveAsync(_rabbitMQOptions.QueueName, cancellationToken);
        }        
        catch (RabbitMQ.Client.Exceptions.AlreadyClosedException)
        {
            _channel?.Dispose();
            _connection?.Dispose();
            _connection = null;
            _channel = null;
            await Task.Delay(_queueExistCheckWaitBetweenIterationMilliSecond, cancellationToken);
            await DeclareQueue(cancellationToken);
        }
        catch (RabbitMQ.Client.Exceptions.OperationInterruptedException)
        {
            await Task.Delay(_queueExistCheckWaitBetweenIterationMilliSecond, cancellationToken);
            await DeclareQueue(cancellationToken);
        }
    }

    public async Task StartProcessingAsync(CancellationToken cancellationToken)
    {        
        await DeclareQueue(cancellationToken);

        if (_channel == null)
            return;

        await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += ProcessMessageAsync;
        await _channel.BasicConsumeAsync(_rabbitMQOptions.QueueName, autoAck: false, consumer: consumer, cancellationToken);
    }

    private async Task ProcessMessageAsync(object sender, BasicDeliverEventArgs args)
    {
        if (_channel == null)
            return;

        try
        {
            if (args.BasicProperties.Headers == null)
            {
                _logger.LogError("Unable to find message details.");
                await _channel.BasicAckAsync(deliveryTag: args.DeliveryTag, multiple: false, args.CancellationToken);
                return;
            }

            if (!args.BasicProperties.Headers.TryGetValue(OutboxMessageProprties.EventType, out object? eventType))
            {
                _logger.LogError("Unable to find event type in message.");
                await _channel.BasicAckAsync(deliveryTag: args.DeliveryTag, multiple: false, args.CancellationToken);
                return;
            }

            if (!_typeCache.TryGetValue(eventType?.ToString()!, out Type? messageType))
            {
                _logger.LogError("Unable to find event type for {EventType}", eventType);
                await _channel.BasicNackAsync(deliveryTag: args.DeliveryTag, multiple: false, requeue: false, cancellationToken: args.CancellationToken);
                return;
            }

            var body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var domainEvent = JsonSerializer.Deserialize(message, messageType!);
            if (domainEvent == null)
            {
                _logger.LogError("Message paylod is empty or unable to convert to {MessageType}", messageType);
                await _channel.BasicAckAsync(deliveryTag: args.DeliveryTag, multiple: false, args.CancellationToken);
                return;
            }

            //TODO - check if message already processed to avoid duplicate processing

            using var scope = _serviceScopeFactory.CreateScope();
            var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();
            await publisher.Publish(domainEvent, args.CancellationToken);

            await _channel.BasicAckAsync(deliveryTag: args.DeliveryTag, multiple: false, args.CancellationToken);

            //TODO - Add processed id
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while processing message {MessageId}", args.BasicProperties.MessageId);
            await _channel.BasicNackAsync(deliveryTag: args.DeliveryTag, multiple: false, requeue: true, cancellationToken: args.CancellationToken);
        }
    }
}