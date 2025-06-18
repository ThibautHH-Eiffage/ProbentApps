using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProbentApps.Database.Contexts;
using ProbentApps.Model;

namespace ProbentApps.Services.Database;

public static class IHostExtensions
{
    public static async Task MigrateDatabaseAsync(this IHost host)
    {
        if (!EF.IsDesignTime)
            return;

        await using var scope = host.Services.CreateAsyncScope();

        await scope.ServiceProvider.MigrateDatabaseAsync<DataProtectionDbContext>();
        await scope.ServiceProvider.MigrateDatabaseAsync<IdentityDbContext>();
        await scope.ServiceProvider.MigrateDatabaseAsync<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var rootUser = (await userManager.FindByIdAsync(ApplicationUser.RootId.ToString()))!;
        await userManager.RemovePasswordAsync(rootUser);
        await userManager.AddPasswordAsync(rootUser, "Test123!");
    }
}
