using Microsoft.Extensions.DependencyInjection;
using ProbentApps.Model;
using ProbentApps.Services.Data.Abstractions;

namespace ProbentApps.Services.Data;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services) => services
        .AddScoped<IAffairRepository, AffairRepository>()
        .AddScoped<IRepository<Affair>, AffairRepository>()
        .AddScoped<IAdvancementManager, AdvancementManager>()
        .AddScoped<IRepository<Advancement>, AdvancementManager>()
        .AddScoped<IReportManager, ReportManager>()
        .AddScoped<IRepository<Report>, ReportManager>()
        .AddScoped<IInvoicingManager, InvoicingManager>()
        .AddScoped<IRepository<Invoice>, InvoicingManager>();
}
