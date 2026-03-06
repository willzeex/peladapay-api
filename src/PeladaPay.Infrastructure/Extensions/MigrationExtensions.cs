using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PeladaPay.Infrastructure.Data;

namespace PeladaPay.Infrastructure.Extensions;

public static class MigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var retries = 5;

        while (retries > 0)
        {
            try
            {
                await db.Database.MigrateAsync();
                break;
            }
            catch
            {
                retries--;
                await Task.Delay(5000);
            }
        }
    }
}
