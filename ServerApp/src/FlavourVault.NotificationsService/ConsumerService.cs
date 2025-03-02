using FlavourVault.NotificationsService.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FlavourVault.NotificationsService;

public class ConsumerService: BackgroundService
{
    private readonly ILogger<ConsumerService> _logger;
    private readonly IMessageConsumer _consumer;

    public ConsumerService(ILogger<ConsumerService> logger, IMessageConsumer consumer)
    {
        _logger = logger;
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _consumer.StartProcessingAsync(stoppingToken);
    }    
}
