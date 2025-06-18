using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ProbentApps.Database.Contexts;

namespace ProbentApps.Services.Database;

internal static class IServiceCollectionExtensions
{
    private static void ConfigureDbContext<TContext>(this IHostApplicationBuilder builder, DbContextOptionsBuilder options) where TContext : IDbContext =>
            ((IDbContextOptionsBuilderInfrastructure)
                options.UseSqlServer(builder.Configuration.GetConnectionString(nameof(ProbentApps))
                    ?? throw new InvalidOperationException(@$"{nameof(ProbentApps)}: No such connection string"),
                    options => options.MigrationsHistoryTable("MigrationsHistory", TContext.Schema))
                .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
                .ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.SensitiveDataLoggingEnabledWarning)))
            .AddOrUpdateExtension(ProbentAppsDbContextOptionsExtension.Instance);

    public static IServiceCollection AddDatabaseServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<DataProtectionDbContext>(builder.ConfigureDbContext<DataProtectionDbContext>)
            .AddDbContext<IdentityDbContext, IdentityDbContext<IdentityDbFunctions>>(builder.ConfigureDbContext<IdentityDbContext>)
            .AddIdentityMigrationsDbContext(options => builder.ConfigureDbContext<IdentityDbContext>(options.UseAsyncSeeding(DataSeeding.SeedIdentityAsync)))
            .AddDbContextFactory<ApplicationDbContext>(builder.ConfigureDbContext<ApplicationDbContext>);

        if (builder.Environment.IsDevelopment())
            builder.Services.AddDatabaseDeveloperPageExceptionFilter()
                .ConfigureOptions<DeveloperDatabaseErrorPageConfiguration>();

        return builder.Services;
    }
}
