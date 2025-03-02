using Azure.Messaging.ServiceBus;
using FlavourVault.Notification.Contracts;
using FlavourVault.NotificationsService.Interfaces;
using FlavourVault.SharedCore.Constants;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FlavourVault.NotificationsService.Consumers;
internal sealed class AzureServiceBusConsumer : IMessageConsumer
{
    private readonly ILogger<AzureServiceBusConsumer> _logger;
    private readonly ServiceBusClient _serviceBusClient;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly Dictionary<string, Type> _typeCache = new()
    {
        { "SMS", typeof(SmsNotification) },
        { "Email", typeof(SmsNotification) }
    };

    public AzureServiceBusConsumer(ILogger<AzureServiceBusConsumer> logger, ServiceBusClient serviceBusClient, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceBusClient = serviceBusClient;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task StartProcessingAsync(CancellationToken cancellationToken)
    {
        var options = new ServiceBusProcessorOptions()
        {
            Identifier = "NotificationsConsumer",
            MaxConcurrentCalls = 1,
            AutoCompleteMessages = false,
            ReceiveMode = ServiceBusReceiveMode.PeekLock            
        };

        var processor = _serviceBusClient.CreateProcessor("Notification", "NotificationSubscription", options);
        processor.ProcessMessageAsync += ProcessMessageAsync;
        processor.ProcessErrorAsync += ProcessErrorAsync;
        await processor.StartProcessingAsync(cancellationToken);
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        try
        {        
            if(!args.Message.ApplicationProperties.TryGetValue(OutboxMessageProprties.EventType, out object? eventType))
            {
                _logger.LogError("Unable to find event type in message.");
                await args.CompleteMessageAsync(args.Message, args.CancellationToken);
                return;
            }

            if (!_typeCache.TryGetValue(eventType.ToString()!, out Type? messageType))
            {
                _logger.LogError("Unable to find event type for {EventType}", eventType);
                await args.CompleteMessageAsync(args.Message, args.CancellationToken);
                return;
            }

            var domainEvent = JsonSerializer.Deserialize(args.Message.Body, messageType!);
            if (domainEvent == null)
            {
                _logger.LogError("Message paylod is empty or unable to convert to {MessageType}", messageType);
                await args.CompleteMessageAsync(args.Message, args.CancellationToken);
                return;
            }

            //TODO - check if message already processed to avoid duplicate processing

            using var scope = _serviceScopeFactory.CreateScope();
            var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();
            await publisher.Publish(domainEvent, args.CancellationToken);
            await args.CompleteMessageAsync(args.Message);

            //TODO - Add processed id
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while processing message {MessageId}", args.Message.MessageId);
            await args.AbandonMessageAsync(args.Message);
        }
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogWarning("Error Processing {@Error}",
            new
            {
                args.Identifier,
                ErrorSource = $"{args.ErrorSource}",
                Exception = $"{args.Exception}"
            });

        return Task.CompletedTask;
    }
}