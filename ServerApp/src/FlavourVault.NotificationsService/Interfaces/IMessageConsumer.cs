namespace FlavourVault.NotificationsService.Interfaces;
public interface IMessageConsumer
{
    Task StartProcessingAsync(CancellationToken cancellationToken);    
}