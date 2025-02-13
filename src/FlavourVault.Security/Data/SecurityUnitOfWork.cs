using FlavourVault.SharedCore.Data;

namespace FlavourVault.Security.Data;

internal sealed class SecurityUnitOfWork : UnitOfWork, ISecurityUnitOfWork
{
    public SecurityUnitOfWork(SecurityDbContext dbContext) : base(dbContext)
    {
    }
}
