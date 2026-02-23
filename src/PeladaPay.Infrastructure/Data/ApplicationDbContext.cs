using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PeladaPay.Domain.Entities;

namespace PeladaPay.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<FinancialAccount> FinancialAccounts => Set<FinancialAccount>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Group>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.HasOne(x => x.FinancialAccount).WithMany().HasForeignKey(x => x.FinancialAccountId);
            entity.HasOne(x => x.Manager).WithMany().HasForeignKey(x => x.ManagerId);
        });

        builder.Entity<Player>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Phone).HasMaxLength(20).IsRequired();
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
        });
    }
}
