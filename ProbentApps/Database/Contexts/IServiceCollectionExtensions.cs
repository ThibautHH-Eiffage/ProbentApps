using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace ProbentApps.Database.Contexts;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddConventionSetPlugins(this IServiceCollection services)
    {
        return services
            .AddScoped<IConventionSetPlugin, ValueGenerationStrategyConventionRemoverConventionSetPlugin>();
    }
}
