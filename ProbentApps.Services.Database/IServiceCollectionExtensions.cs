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
    private static void ConfigureDbContext<TContext>(this IHostApplicationBuilder builder, DbContextOptionsBuilder options)
        where TContext : DbContext, IDbContext =>
            builder.ConfigureDbContext<TContext>(options, nameof(ProbentApps));
    private static void ConfigureDbContext<TContext>(this IHostApplicationBuilder builder, DbContextOptionsBuilder options, string connectionString)
        where TContext : DbContext, IDbContext =>
            ((IDbContextOptionsBuilderInfrastructure)
                options.UseSqlServer(builder.Configuration.GetConnectionString(connectionString)
                    ?? throw new InvalidOperationException(connectionString + ": No such connection string"),
                    options => options.MigrationsHistoryTable("MigrationsHistory", TContext.Schema))
                .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
                .ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.SensitiveDataLoggingEnabledWarning)))
            .AddOrUpdateExtension(ProbentAppsDbContextOptionsExtension.Instance);

    public static IServiceCollection AddDatabaseServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<DataProtectionDbContext>(builder.ConfigureDbContext<DataProtectionDbContext>)
            .AddDbContext<IdentityDbContext, IdentityDbContext<IdentityDbFunctions>>(builder.ConfigureDbContext<IdentityDbContext>)
            .AddIdentityMigrationsDbContext(builder.ConfigureDbContext<IdentityDbContext>)
            .AddDbContextFactory<ApplicationDbContext>(builder.ConfigureDbContext<ApplicationDbContext>);

        if (builder.Environment.IsDevelopment())
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        return builder.Services;
    }
}
