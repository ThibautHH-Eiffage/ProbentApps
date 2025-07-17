using Microsoft.Extensions.DependencyInjection;

namespace ProbentApps.Services.Options;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection ConfigureOptions(this IServiceCollection services) => services
        /*.ConfigureOptions<ApplicationStaticFilesOptionsConfiguration>()*/;
}
