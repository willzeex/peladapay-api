using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeladaPay.Domain.Entities;

namespace PeladaPay.Infrastructure.Data.Configurations;

public sealed class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> entity)
    {
        entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
        entity.Property(x => x.Frequency).HasMaxLength(30);
        entity.Property(x => x.Venue).HasMaxLength(120);
        entity.Property(x => x.CrestUrl).HasMaxLength(500);

        entity.HasOne(x => x.FinancialAccount)
            .WithMany()
            .HasForeignKey(x => x.FinancialAccountId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(x => x.Organizer)
            .WithMany(x => x.OrganizedGroups)
            .HasForeignKey(x => x.OrganizerId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasMany(x => x.Players)
            .WithMany(x => x.Groups)
            .UsingEntity<GroupPlayer>(
                right => right
                    .HasOne(x => x.Player)
                    .WithMany(x => x.GroupPlayers)
                    .HasForeignKey(x => x.PlayerId)
                    .OnDelete(DeleteBehavior.Restrict),
                left => left
                    .HasOne(x => x.Group)
                    .WithMany(x => x.GroupPlayers)
                    .HasForeignKey(x => x.GroupId)
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.HasKey(x => x.Id);
                    join.HasIndex(x => new { x.GroupId, x.PlayerId }).IsUnique();
                });
    }
}
