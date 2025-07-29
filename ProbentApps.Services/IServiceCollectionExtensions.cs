using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProbentApps.Services.Components;
using ProbentApps.Services.Data;
using ProbentApps.Services.Database;
using ProbentApps.Services.Email;
using ProbentApps.Services.Identity;
using ProbentApps.Services.Options;

namespace ProbentApps.Services;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IHostEnvironment environment, IConfigurationManager configuration) => services
        .AddDatabaseServices(environment, configuration)
        .AddEmailServices()
        .AddIdentityServices()
        .AddDataServices()
        .AddComponentServices()
        .ConfigureOptions();
}
