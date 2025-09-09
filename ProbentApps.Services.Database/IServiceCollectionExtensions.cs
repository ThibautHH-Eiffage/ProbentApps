using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProbentApps.Services.Database.Abstractions.Contexts;

namespace ProbentApps.Services.Database;

public static class IServiceCollectionExtensions
{
    private static string GetRequiredConnectionString(this IConfiguration configuration, string name) => configuration.GetConnectionString(name)
        ?? throw new InvalidOperationException(name + ": No such connection string");

    private static DbContextOptionsBuilder UseEnvironmentConfiguration(this DbContextOptionsBuilder options, IHostEnvironment environment) => options
        .EnableSensitiveDataLogging(environment.IsDevelopment())
        .ConfigureWarnings(warnings =>
		{
			if (environment.IsDevelopment())
				warnings.Ignore(CoreEventId.SensitiveDataLoggingEnabledWarning);
			warnings.Ignore(RelationalEventId.MultipleCollectionIncludeWarning);
		});

    private static void ConfigureDbContext<TContext>(this DbContextOptionsBuilder options, IHostEnvironment environment, IConfiguration configuration)
        where TContext : DbContext, IDbContext =>
            ConfigureDbContext<TContext>(options.UseEnvironmentConfiguration(environment),
                configuration.GetRequiredConnectionString(nameof(ProbentApps)));

    private static void ConfigureDbContext<TContext>(DbContextOptionsBuilder options, string connectionString)
        where TContext : DbContext, IDbContext =>
            ((IDbContextOptionsBuilderInfrastructure)
                options.UseSqlServer(connectionString, builder => builder.MigrationsHistoryTable("MigrationsHistory", TContext.Schema)))
            .AddOrUpdateExtension(ProbentAppsDbContextOptionsExtension.Instance);

    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IHostEnvironment environment, IConfiguration configuration) => services
        .AddDbContext<DataProtectionDbContext>(o => o.ConfigureDbContext<DataProtectionDbContext>(environment, configuration))
        .AddDbContext<IdentityDbContext, IdentityDbContext<IdentityDbFunctions>>(o => o.ConfigureDbContext<IdentityDbContext>(environment, configuration))
        .AddIdentityMigrationsDbContext(o => o.ConfigureDbContext<IdentityDbContext>(environment, configuration))
        .AddDbContextFactory<ApplicationDbContext>(o => o.ConfigureDbContext<ApplicationDbContext>(environment, configuration));
}
