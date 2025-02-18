using FlavourVault.OutboxDispatcher.Interfaces;
using FlavourVault.Results;
using FlavourVault.SharedCore.Data;
using FlavourVault.SharedCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;

namespace FlavourVault.OutboxDispatcher;

public sealed class OutboxDispatcherService<TContext>(    
    ILogger<OutboxDispatcherService<TContext>> logger,
    IServiceScopeFactory serviceScopeProxy,
    IDomainEventDispatcher domainEventDispatcher,
    IMessageBusDispatcher messageBusDispatcher) : BackgroundService where TContext: OutboxDbContextBase
{    
    private const int _waitBetweenIterationMilliSecond = 10_00;     
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Outbox Dispatcher Service starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = serviceScopeProxy.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();

            try
            {
                using (var transaction = await dbContext.Database.BeginTransactionAsync(stoppingToken))
                {
                    //Retrieve records and lock them so other processes can't process same message
                    //Where retry is needed only process where next retry date is in past

                    var messages = await dbContext.OutboxMessages.FromSqlRaw(@$"
            SELECT TOP 50 *
            FROM [{dbContext.SchemaName}].[OutboxMessages] WITH (ROWLOCK, UPDLOCK, READPAST)
            WHERE Status = 'PENDING'
            AND (NextRetryDate Is Null Or NextRetryDate<GetUtcDate())
            ORDER BY EventDate Asc")
                        .ToListAsync(stoppingToken);

                    foreach (var message in messages)
                    {
                        try
                        {
                            Result result;
                            if (!string.IsNullOrEmpty(message.TopicOrQueueName))
                            {
                                result = await messageBusDispatcher.DispatchAsync(message, stoppingToken);                                
                            }
                            else
                            {
                                result = await domainEventDispatcher.DispatchAsync(message, stoppingToken);                                
                            }

                            if (!result.IsSuccess)
                            {
                                logger.LogError(result.Errors?.FirstOrDefault()?.ErrorMessage);
                                continue;
                            }

                            message.MarkAsProcessed();
                        }
                        catch (Exception ex) when (ex is ArgumentNullException || ex is NotImplementedException)
                        {
                            message.MarkAsFailed(ex.Message);
                        }                        
                        catch (BrokenCircuitException ex)
                        {
                            logger.LogError("Circuit is open");
                            message.MarkForRetryOrFail(ex.Message, true);
                        }
                        catch (Exception ex)
                        {
                            message.MarkForRetryOrFail(ex.Message, false);
                            logger.LogError(ex.Message);
                        }
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
                    await transaction.CommitAsync(stoppingToken);
                }                
            }            
            catch (Exception ex)
            {
                logger.LogException(ex);                
            }
            finally
            {
                await Task.Delay(_waitBetweenIterationMilliSecond, stoppingToken);
            }
        }
        logger.LogInformation("Outbox Dispatcher Service stopping.");
    }    
}