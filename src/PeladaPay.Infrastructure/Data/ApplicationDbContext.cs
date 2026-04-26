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
    public DbSet<OnboardingGroupDraft> OnboardingGroupDrafts => Set<OnboardingGroupDraft>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<GroupPlayer> GroupPlayers => Set<GroupPlayer>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<FinancialAccount> FinancialAccounts => Set<FinancialAccount>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<PlanFeature> PlanFeatures => Set<PlanFeature>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
