namespace FlavourVault.SharedCore.Data;
public interface IUnitOfWork
{
    Task Begin(CancellationToken cancellationToken = default);
    Task Commit(CancellationToken cancellationToken = default);
    Task Rollback(CancellationToken cancellationToken = default);        
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}