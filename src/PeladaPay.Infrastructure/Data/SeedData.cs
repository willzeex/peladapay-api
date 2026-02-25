using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Enums;

namespace PeladaPay.Infrastructure.Data;

public static class SeedData
{
    public static async Task EnsureSeedDataAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        await context.Database.MigrateAsync();

        const string managerEmail = "gestor@peladapay.com";
        const string organizerRole = "Organizer";
        var manager = await userManager.FindByEmailAsync(managerEmail);

        if (!await roleManager.RoleExistsAsync(organizerRole))
        {
            await roleManager.CreateAsync(new IdentityRole(organizerRole));
        }

        if (manager is null)
        {
            manager = new ApplicationUser
            {
                UserName = managerEmail,
                Email = managerEmail,
                FullName = "Gestor Teste"
            };

            await userManager.CreateAsync(manager, "Pelada123!");
        }

        if (!await userManager.IsInRoleAsync(manager, organizerRole))
        {
            await userManager.AddToRoleAsync(manager, organizerRole);
        }

        if (await context.Groups.AnyAsync())
        {
            return;
        }

        var account = new FinancialAccount
        {
            PixKey = "gestor@pix.com",
            ExternalSubaccountId = "subacc_seed_001",
            Balance = 350
        };

        var group = new Group
        {
            Name = "Pelada de Quinta",
            MatchDate = DateTime.UtcNow.AddDays(2),
            OrganizerId = manager.Id,
            FinancialAccount = account
        };

        context.Groups.Add(group);
        context.Transactions.AddRange(
            new Transaction
            {
                Group = group,
                Amount = 200,
                DateUtc = DateTime.UtcNow.AddDays(-2),
                Type = TransactionType.Entrada,
                Status = TransactionStatus.Confirmada,
                Description = "Mensalidade João"
            },
            new Transaction
            {
                Group = group,
                Amount = 150,
                DateUtc = DateTime.UtcNow.AddDays(-1),
                Type = TransactionType.Entrada,
                Status = TransactionStatus.Confirmada,
                Description = "Mensalidade Pedro"
            },
            new Transaction
            {
                Group = group,
                Amount = 50,
                DateUtc = DateTime.UtcNow,
                Type = TransactionType.Saida,
                Status = TransactionStatus.Pendente,
                Description = "Compra de coletes"
            });

        await context.SaveChangesAsync();
    }
}
