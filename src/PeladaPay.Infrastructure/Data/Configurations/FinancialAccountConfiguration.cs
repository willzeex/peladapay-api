using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeladaPay.Domain.Entities;

namespace PeladaPay.Infrastructure.Data.Configurations;

public sealed class FinancialAccountConfiguration : IEntityTypeConfiguration<FinancialAccount>
{
    public void Configure(EntityTypeBuilder<FinancialAccount> entity)
    {
        entity.Property(x => x.Balance).HasPrecision(18, 2);
        entity.Property(x => x.PixKey).HasMaxLength(120).IsRequired();
        entity.Property(x => x.ExternalSubaccountId).HasMaxLength(80).IsRequired();
        entity.Property(x => x.MonthlyFee).HasPrecision(18, 2);
        entity.Property(x => x.SingleMatchFee).HasPrecision(18, 2);
    }
}
