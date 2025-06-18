using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProbentApps.Model;
using ProbentApps.Services.Database.Abstractions.Contexts;

namespace ProbentApps.Services.Database;

public static class IHostExtensions
{
    public static async Task MigrateDatabaseAsync(this IHost host)
    {
        if (EF.IsDesignTime)
            return;

        await using var scope = host.Services.CreateAsyncScope();

        await scope.ServiceProvider.MigrateDatabaseAsync<DataProtectionDbContext>();
        await scope.ServiceProvider.MigrateDatabaseAsync<IdentityDbContext>();
        await scope.ServiceProvider.MigrateDatabaseAsync<ApplicationDbContext>();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var rootUser = await userManager.FindByIdAsync(ApplicationUser.RootId.ToString());
        var password = "Test123!";
        if (rootUser is null)
        {
            await userManager.CreateAsync(new ApplicationUser
            {
                Id = ApplicationUser.RootId,
                UserName = "root",
                Email = "root@apps.probent.com",
                EmailConfirmed = true,
                ManagedStructures = null!
            }, password);
        } else
        {
            await userManager.RemovePasswordAsync(rootUser);
            await userManager.AddPasswordAsync(rootUser, password);
        }
    }
}
