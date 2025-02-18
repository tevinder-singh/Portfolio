using FlavourVault.Audit.Domain.AuditTrail;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;
using FlavourVault.SharedCore.Domain.DomainEvents;

namespace FlavourVault.Audit.Data.EntitiesConfiguration;

internal class AuditTrailConfiguration : IEntityTypeConfiguration<AuditTrail>
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

    public void Configure(EntityTypeBuilder<AuditTrail> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => new { e.RecordId, e.Area, e.Category });
        builder.Property(e => e.UserName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Area).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Category).HasMaxLength(200).IsRequired();        
        builder.Property(e => e.CreatedOn).IsRequired();
        builder.Property(e => e.Action).HasMaxLength(200).IsRequired();
        builder.Property(e => e.ActionKey).HasMaxLength(200).IsRequired();

        builder.Property(e => e.ChangedValues)
            .HasConversion(
            v => JsonSerializer.Serialize(v, _jsonSerializerOptions),
            v => JsonSerializer.Deserialize<ICollection<AuditTrailChangeValue>>(v, _jsonSerializerOptions)
            );
    }
}
