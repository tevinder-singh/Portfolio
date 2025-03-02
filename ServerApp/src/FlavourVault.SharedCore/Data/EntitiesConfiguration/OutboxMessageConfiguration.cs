using FlavourVault.SharedCore.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlavourVault.SharedCore.Data.EntitiesConfiguration;
internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.HasKey(e => e.Id);        
        builder.Property(e => e.EventType).HasMaxLength(200).IsRequired();

        builder.HasIndex(e => e.EventDate);
    }
}