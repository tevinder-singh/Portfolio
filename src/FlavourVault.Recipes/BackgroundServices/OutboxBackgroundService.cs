using FlavourVault.SharedCore.Extensions;

namespace FlavourVault.Recipes.BackgroundServices;

internal class OutboxBackgroundService : BackgroundService
{
    private readonly ILogger<OutboxBackgroundService> _logger;
    private readonly IOutboxProcessor _outboxProcessor;
    private const int _waitBetweenIterationMilliSecond = 10_00;
    public OutboxBackgroundService(IOutboxProcessor outboxProcessor, ILogger<OutboxBackgroundService> logger)
    {
        _logger = logger;
        _outboxProcessor = outboxProcessor;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Recipes OutboxBackgroundService starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _outboxProcessor.ProcessPendingNotifications();
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
            finally
            {
                await Task.Delay(_waitBetweenIterationMilliSecond, stoppingToken);
            }
        }
        _logger.LogInformation("Recipes OutboxBackgroundService stopping.");
    }
}
