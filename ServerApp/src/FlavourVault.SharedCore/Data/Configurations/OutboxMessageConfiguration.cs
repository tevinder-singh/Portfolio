using FlavourVault.SharedCore.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlavourVault.SharedCore.Data.Configurations;
internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.HasKey(e => e.Id);        
        builder.Property(e => e.EventType).HasMaxLength(200).IsRequired();
        builder.Property(e => e.TopicOrQueueName).HasMaxLength(200);
        builder.Property(e => e.RecordType).HasMaxLength(200);
        builder.Property(e => e.EventDate).IsRequired();
        builder.Property(e => e.Status).HasMaxLength(10).IsRequired();

        builder.HasIndex(e => e.EventDate);
    }
}