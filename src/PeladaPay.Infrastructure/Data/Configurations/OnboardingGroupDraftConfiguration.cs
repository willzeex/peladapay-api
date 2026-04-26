using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Enums;

namespace PeladaPay.Infrastructure.Data.Configurations;

public sealed class OnboardingGroupDraftConfiguration : IEntityTypeConfiguration<OnboardingGroupDraft>
{
    public void Configure(EntityTypeBuilder<OnboardingGroupDraft> entity)
    {
        entity.Property(x => x.UserId).IsRequired();
        entity.Property(x => x.Name).HasMaxLength(100);
        entity.Property(x => x.Frequency)
            .HasConversion(
                frequency => frequency.HasValue ? frequency.Value.ToString() : null,
                value => string.IsNullOrWhiteSpace(value)
                    ? null
                    : Enum.Parse<GroupFrequency>(value, true))
            .HasMaxLength(30);
        entity.Property(x => x.Venue).HasMaxLength(120);
        entity.Property(x => x.CrestUrl).HasMaxLength(500);

        entity.HasIndex(x => x.UserId).IsUnique();

        entity.HasOne(x => x.User)
            .WithOne(x => x.OnboardingGroupDraft)
            .HasForeignKey<OnboardingGroupDraft>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
