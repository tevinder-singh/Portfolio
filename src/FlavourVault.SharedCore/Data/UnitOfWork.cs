using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FlavourVault.SharedCore.Data;
public abstract class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _dbContext;
    private IDbContextTransaction? _dbTransaction;

    protected UnitOfWork(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Begin(CancellationToken cancellationToken = default)
    {
        _dbTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);            
    }

    public async Task Commit(CancellationToken cancellationToken = default)
    {
        if(_dbTransaction!=null)
            await _dbTransaction.CommitAsync(cancellationToken);
    }

    public async Task Rollback(CancellationToken cancellationToken = default)
    {
        if (_dbTransaction != null)            
            await _dbTransaction.RollbackAsync(cancellationToken);
    }

    public virtual async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }        
}
