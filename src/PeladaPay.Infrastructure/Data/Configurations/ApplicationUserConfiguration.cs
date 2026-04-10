using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeladaPay.Domain.Entities;

namespace PeladaPay.Infrastructure.Data.Configurations;

public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> entity)
    {
        entity.Property(x => x.Whatsapp).HasMaxLength(20);
        entity.Property(x => x.Cpf).HasMaxLength(14);
        entity.Property(x => x.Address).HasMaxLength(200);
        entity.Property(x => x.OnboardingGroupName).HasMaxLength(100);
        entity.Property(x => x.OnboardingFrequency)
            .HasConversion(
                frequency => frequency.HasValue ? frequency.Value.ToString() : null,
                value => string.IsNullOrWhiteSpace(value)
                    ? null
                    : Enum.Parse<PeladaPay.Domain.Enums.GroupFrequency>(value, true))
            .HasMaxLength(30);
        entity.Property(x => x.OnboardingVenue).HasMaxLength(120);
        entity.Property(x => x.OnboardingCrestUrl).HasMaxLength(500);

        entity.HasOne(x => x.Plan)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.PlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
