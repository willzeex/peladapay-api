using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PeladaPay.Domain.Entities;
using PeladaPay.Infrastructure.Data.Configurations;

namespace PeladaPay.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<GroupPlayer> GroupPlayers => Set<GroupPlayer>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<FinancialAccount> FinancialAccounts => Set<FinancialAccount>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Group>(entity =>
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
        });

        builder.ApplyConfiguration(new PlayerConfiguration());

        builder.Entity<GroupPlayer>(entity =>
        {
            entity.ToTable("GroupPlayers");
        });

        builder.Entity<Transaction>(entity =>
        {
            entity.Property(x => x.Amount).HasPrecision(18, 2);
            entity.Property(x => x.Description).HasMaxLength(150);
        });

        builder.Entity<FinancialAccount>(entity =>
        {
            entity.Property(x => x.Balance).HasPrecision(18, 2);
            entity.Property(x => x.PixKey).HasMaxLength(120).IsRequired();
            entity.Property(x => x.ExternalSubaccountId).HasMaxLength(80).IsRequired();
            entity.Property(x => x.MonthlyFee).HasPrecision(18, 2);
            entity.Property(x => x.SingleMatchFee).HasPrecision(18, 2);
        });

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(x => x.Whatsapp).HasMaxLength(20);
            entity.Property(x => x.Cpf).HasMaxLength(14);
            entity.Property(x => x.Address).HasMaxLength(200);
            entity.Property(x => x.OnboardingGroupName).HasMaxLength(100);
            entity.Property(x => x.OnboardingFrequency).HasMaxLength(30);
            entity.Property(x => x.OnboardingVenue).HasMaxLength(120);
            entity.Property(x => x.OnboardingCrestUrl).HasMaxLength(500);
        });

    }
}
