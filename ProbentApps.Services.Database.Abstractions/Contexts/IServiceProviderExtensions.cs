using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ProbentApps.Services.Database.Abstractions.Contexts;

public static class IServiceProviderExtensions
{
    private static TContext? GetKeyedMigrationsDbContext<TContext>(this IServiceProvider serviceProvider) where TContext : DbContext =>
        serviceProvider.GetKeyedService<TContext>(IServiceCollectionExtensions.MigrationsDbContextServiceKey);

    public static TContext GetRequiredMigrationsDbContext<TContext>(this IServiceProvider serviceProvider) where TContext : DbContext =>
        serviceProvider.GetKeyedMigrationsDbContext<TContext>() ?? serviceProvider.GetRequiredService<TContext>();

    public static TContext? GetMigrationsDbContext<TContext>(this IServiceProvider serviceProvider) where TContext : DbContext =>
        serviceProvider.GetKeyedMigrationsDbContext<TContext>() ?? serviceProvider.GetService<TContext>();

    public static Task MigrateDatabaseAsync<TContext>(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default) where TContext : DbContext =>
        serviceProvider.GetRequiredMigrationsDbContext<TContext>().Database.MigrateAsync(cancellationToken);
}
