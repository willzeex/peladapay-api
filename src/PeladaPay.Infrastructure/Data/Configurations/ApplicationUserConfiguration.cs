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

        entity.HasOne(x => x.Plan)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.PlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
