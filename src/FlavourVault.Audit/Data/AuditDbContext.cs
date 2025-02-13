using FlavourVault.Audit.Domain.AuditTrail;
using Microsoft.EntityFrameworkCore;

namespace FlavourVault.Recipes.Data;

internal sealed class AuditDbContext: DbContext
{
    public AuditDbContext(DbContextOptions<AuditDbContext> dbContextOptions) : base(dbContextOptions)
    {            
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {        
        modelBuilder.HasDefaultSchema(SchemaName);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuditDbContext).Assembly);        
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<AuditTrail> AuditTrails { get; set; }

    public const string SchemaName = "Audit";
}