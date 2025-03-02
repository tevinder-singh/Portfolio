using FlavourVault.SharedCore.Domain.Common;
using FlavourVault.SharedCore.Domain.DomainEvents;
using FlavourVault.SharedCore.Domain.Enums;
using FlavourVault.SharedCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlavourVault.SharedCore.Data;
public abstract class DbContextBase : DbContext
{
    private readonly IUserContext _userContext;
    private readonly string _schemaName;
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
    private static readonly string[] IgnoredProperties = ["Password", "CreatedBy", "CreatedOn", "ModifiedOn", "ModifiedBy"];

    protected DbContextBase(DbContextOptions dbContextOptions, IUserContext userContext, string schemaName) : base(dbContextOptions)
    {
        _userContext = userContext;
        _schemaName = schemaName;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (!string.IsNullOrEmpty(_schemaName))
            modelBuilder.HasDefaultSchema(_schemaName);
                
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DbContextBase).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        Guid? userId= _userContext.GetUserId();
        string userName = _userContext.GetUserName() ?? "System";

        SetAuditableProperties(userId);
        await CreateAuditTrailNotifications(userName, cancellationToken);
        await CreateDomainEventNotifications(cancellationToken);

        return await base.SaveChangesAsync(cancellationToken);
    }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    #region domain events

    private async Task CreateDomainEventNotifications(CancellationToken cancellationToken = default)
    {        
        foreach (var entity in ChangeTracker.Entries<IHasDomainEvent>().Select(x=>x.Entity))
        {
            foreach (DomainEvent domainEvent in entity.DomainEvents.Where(x=>!x.IsPublished))
            {
                var outboxMessage = new OutboxMessage
                (
                    domainEvent.EventType ?? domainEvent.GetType().FullName!,
                    entity.GetType().Name,
                    JsonSerializer.Serialize(domainEvent, _jsonSerializerOptions),
                    domainEvent.Priority
                )
                {
                    RecordId = entity.Id,
                    TopicOrQueueName = domainEvent.TopicOrQueueName,
                    CorrelationId = domainEvent.CorrelationId,
                };
                await AddAsync(outboxMessage, cancellationToken);
                domainEvent.IsPublished = true;
            }
        }
    }

    #endregion

    #region auditing

    private void SetAuditableProperties(Guid? userId)
    {
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedOn = DateTime.UtcNow;
                    entry.Entity.CreatedBy = userId;
                    break;

                case EntityState.Modified:
                    ResetIsModifiedFlag(entry, "CreatedOn");
                    ResetIsModifiedFlag(entry, "CreatedBy");
                    entry.Entity.ModifiedOn = DateTime.UtcNow;
                    entry.Entity.ModifiedBy = userId;
                    break;
            }
        }
    }

    private async Task CreateAuditTrailNotifications(string userName, CancellationToken cancellationToken = default)
    {
        var auditableEntries = ChangeTracker.Entries<IAuditableEntity>()
            .Where(x => x.State is EntityState.Added or EntityState.Deleted or EntityState.Modified)
            .Select(x => CreateTrailEntry(userName, x))
            .ToList();

        foreach (var item in auditableEntries)
        {
            var outboxMessage = new OutboxMessage
            (
                item.GetType().FullName,
                item.GetType().Name,
                JsonSerializer.Serialize(item, _jsonSerializerOptions),
                EventPriority.Low
            );
            await AddAsync(outboxMessage, cancellationToken);
        }
    }

    private AuditTrailNotification CreateTrailEntry(string username, EntityEntry<IAuditableEntity> entry)
    {        
        var auditTrail = new AuditTrailNotification(username, GetAction(entry), GetActionKey(entry), _schemaName, entry.Entity.GetType().Name, entry.Entity.Id);

        SetAuditTrailPropertyValues(entry, auditTrail);        
        return auditTrail;
    }

    private static string GetAction(EntityEntry<IAuditableEntity> entry)
    {
        return entry.State switch
        {
            EntityState.Deleted => $"{entry.Entity.GetType().Name} Deleted",
            EntityState.Modified => $"{entry.Entity.GetType().Name} Updated",
            EntityState.Added => $"{entry.Entity.GetType().Name} Created",
            _ => $"{entry.Entity.GetType().Name}",
        };
    }

    private static string GetActionKey(EntityEntry<IAuditableEntity> entry)
    {
        return entry.State switch
        {
            EntityState.Deleted => $"{entry.Entity.GetType().FullName}.Deleted",
            EntityState.Modified => $"{entry.Entity.GetType().FullName}.Updated",
            EntityState.Added => $"{entry.Entity.GetType().FullName}.Created",
            _ => $"{entry.Entity.GetType().Name}",
        };
    }

    private static void SetAuditTrailPropertyValues(EntityEntry entry, AuditTrailNotification trailEntry)
    {        
        foreach (var property in entry.Properties.Where(x => !x.IsTemporary))
        {
            if (property.Metadata.IsPrimaryKey())
                continue;            

            // Filter properties that should not appear in the audit list
            if (IgnoredProperties.Contains(property.Metadata.Name))            
                continue;            

            SetAuditTrailPropertyValue(entry, trailEntry, property);
        }
    }

    private static void SetAuditTrailPropertyValue(EntityEntry entry, AuditTrailNotification trailEntry, PropertyEntry property)
    {
        var propertyName = property.Metadata.Name;

        switch (entry.State)
        {
            case EntityState.Modified:
                if (property.IsModified && (property.OriginalValue is null || !property.OriginalValue.Equals(property.CurrentValue)))
                {
                    trailEntry.ChangedValues.Add(new AuditTrailChangeValue(propertyName, property.OriginalValue, property.CurrentValue));                                        
                }
                break;
        }
    }

    private static void ResetIsModifiedFlag(EntityEntry entry, string propertyName)
    {
        var propertyEntry = entry.Properties.FirstOrDefault(x => x.Metadata.Name == propertyName);
        if (propertyEntry != null)
            propertyEntry.IsModified = false;
    }

    #endregion


}