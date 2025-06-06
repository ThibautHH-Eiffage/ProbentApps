using Microsoft.EntityFrameworkCore;
using ProbentApps.Model;

namespace ProbentApps.Services.Database;

internal static class DataSeeding
{
    public static async Task SeedIdentityAsync(this DbContext context, bool storeChanged, CancellationToken cancellationToken = default)
    {
        var users = context.Set<ApplicationUser>();
        if (!await users.AnyAsync(u => u.Id == ApplicationUser.RootId, cancellationToken))
            users.Add(new()
            {
                Id = ApplicationUser.RootId,
                UserName = "root",
                NormalizedUserName = "ROOT",
                Email = "root@apps.probent.local",
                NormalizedEmail = "ROOT@APPS.PROBENT.LOCAL",
                EmailConfirmed = true,
                ManagedStructures = []
            });
        if (!await users.AnyAsync(u => u.Id == Guid.AllBitsSet, cancellationToken))
            users.Add(new() { Id = Guid.AllBitsSet, UserName = "Deleted user", ManagedStructures = [] });
        await context.SaveChangesAsync(cancellationToken);
    }
}
