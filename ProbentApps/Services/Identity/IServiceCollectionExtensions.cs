using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using ProbentApps.Database.Contexts;
using ProbentApps.Model;
using ProbentApps.Services.Data.Identity;

namespace ProbentApps.Services.Identity;

internal static class IServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddDataProtection()
            .SetApplicationName(nameof(ProbentApps))
            .PersistKeysToDbContext<DataProtectionDbContext>();

        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(static options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
            options.Stores.ProtectPersonalData = true;
            options.Stores.SchemaVersion = IdentitySchemaVersions.Version2;
            options.User.RequireUniqueEmail = true;
        })
        .AddPersonalDataProtection<LookupProtector, LookupProtectorKeyRing>()
        .AddEntityFrameworkStores<IdentityDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<UserManager<ApplicationUser>, DataProtectionLookupProtectedUserManager<ApplicationUser>>()
            .AddScoped<IUserStore<ApplicationUser>, DataProtectionLookupProtectedUserStore<ApplicationUser, IdentityRole<Guid>, IdentityDbContext, Guid>>()
            .AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

        return services;
    }
}
