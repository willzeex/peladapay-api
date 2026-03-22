using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeladaPay.Domain.Entities;

namespace PeladaPay.Infrastructure.Data.Configurations;

public sealed class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> entity)
    {
        entity.Property(x => x.Amount).HasPrecision(18, 2);
        entity.Property(x => x.Description).HasMaxLength(150);
        entity.Property(x => x.PaymentLink).HasMaxLength(500);

        entity.HasOne(x => x.Player)
            .WithMany()
            .HasForeignKey(x => x.PlayerId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(x => x.ExternalChargeId).IsUnique();
    }
}
