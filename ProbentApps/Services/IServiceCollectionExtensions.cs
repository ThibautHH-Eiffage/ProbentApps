using ProbentApps.Services.Components;
using ProbentApps.Services.Data;
using ProbentApps.Services.Database;
using ProbentApps.Services.Identity;

namespace ProbentApps.Services;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IHostApplicationBuilder services) => services
        .AddDatabaseServices()
        .AddIdentityServices()
        .AddDataServices()
        .AddComponentServices();
}
