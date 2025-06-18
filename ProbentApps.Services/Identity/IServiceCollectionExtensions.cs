using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ProbentApps.Model;
using ProbentApps.Services.Database.Abstractions.Contexts;
using ProbentApps.Services.Identity.Data;
using System.Diagnostics.CodeAnalysis;

namespace ProbentApps.Services.Identity;

internal static class IServiceCollectionExtensions
{
    private class DummyLookupProtector : ILookupProtector
    {
        internal static readonly DummyLookupProtector Instance = new();

        private DummyLookupProtector() { }

        [return: NotNullIfNotNull(nameof(data))]
        public string? Protect(string keyId, string? data) => data;

        [return: NotNullIfNotNull(nameof(data))]
        public string? Unprotect(string keyId, string? data) => data;
    }

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
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();
        services.AddSingleton<ILookupProtector>(DummyLookupProtector.Instance) // Required dummy service instance for UserManager<TUser> constructor check
            .AddSingleton<IPersonalDataProtector, PersonalDataProtector>();

        services.AddScoped<UserManager<ApplicationUser>, LookupHashingUserManager<ApplicationUser>>()
            .AddScoped<IUserStore<ApplicationUser>, LookupHashingUserStore<ApplicationUser, IdentityRole<Guid>, IdentityDbContext, Guid>>()
            .AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

        return services;
    }
}
