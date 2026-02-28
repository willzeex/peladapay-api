using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeladaPay.Domain.Entities;

namespace PeladaPay.Infrastructure.Data.Configurations;

public sealed class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> entity)
    {
        entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
        entity.Property(x => x.Cpf).HasMaxLength(14).IsRequired();
        entity.Property(x => x.Email).HasMaxLength(120);
        entity.Property(x => x.Phone).HasMaxLength(20).IsRequired();

        entity.HasIndex(x => x.Cpf).IsUnique();
        entity.HasIndex(x => x.Email).IsUnique();
        entity.HasIndex(x => x.Phone).IsUnique();
    }
}
