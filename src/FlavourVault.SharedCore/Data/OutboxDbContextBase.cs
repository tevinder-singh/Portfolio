using FlavourVault.SharedCore.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace FlavourVault.SharedCore.Data;
public abstract class OutboxDbContextBase(DbContextOptions dbContextOptions, string schemaName) : DbContext(dbContextOptions)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {        
        modelBuilder.HasDefaultSchema(SchemaName);                
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OutboxDbContextBase).Assembly);        
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    public string SchemaName { get; private set; } = schemaName;
}