using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeladaPay.Domain.Entities;

namespace PeladaPay.Infrastructure.Data.Configurations;

public sealed class GroupPlayerConfiguration : IEntityTypeConfiguration<GroupPlayer>
{
    public void Configure(EntityTypeBuilder<GroupPlayer> entity)
    {
        entity.ToTable("GroupPlayers");
    }
}
