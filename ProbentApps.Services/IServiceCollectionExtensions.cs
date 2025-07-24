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
    public static IServiceCollection AddApplicationServices(this IHostApplicationBuilder builder) => builder
        .AddDatabaseServices()
        .AddEmailServices()
        .AddIdentityServices()
        .AddDataServices()
        .AddComponentServices()
        .ConfigureOptions();
}
