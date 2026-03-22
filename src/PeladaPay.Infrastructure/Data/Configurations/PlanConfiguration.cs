using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeladaPay.Domain.Constants;
using PeladaPay.Domain.Entities;

namespace PeladaPay.Infrastructure.Data.Configurations;

public sealed class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> entity)
    {
        entity.Property(x => x.Name).HasMaxLength(60).IsRequired();
        entity.Property(x => x.Slug).HasMaxLength(30).IsRequired();
        entity.Property(x => x.MonthlyPrice).HasPrecision(18, 2);
        entity.Property(x => x.PixFeePercentage).HasPrecision(5, 2);
        entity.Property(x => x.AdditionalWithdrawalFee).HasPrecision(18, 2);

        entity.HasIndex(x => x.Slug).IsUnique();

        entity.HasData(
            new Plan
            {
                Id = Plans.FreeId,
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Name = "Plano Grátis",
                Slug = "free",
                MonthlyPrice = 0m,
                PixFeePercentage = 1.90m,
                FreeWithdrawalsPerMonth = 0,
                AdditionalWithdrawalFee = 5.00m,
                IsPopular = false,
                DisplayOrder = 1
            },
            new Plan
            {
                Id = Plans.ProId,
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Name = "Plano Pro",
                Slug = "pro",
                MonthlyPrice = 29.90m,
                PixFeePercentage = 0m,
                FreeWithdrawalsPerMonth = 1,
                AdditionalWithdrawalFee = 5.00m,
                IsPopular = true,
                DisplayOrder = 2
            });
    }
}
