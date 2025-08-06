using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ProbentApps.Services.Generic;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddGenericServices(this IServiceCollection services, IHostEnvironment environment)
    {
        if (environment.IsDevelopment())
            services.AddHttpLogging();
        else if (environment.IsStaging())
            services.AddHttpLogging();
        else;

        return services;
    }
}
