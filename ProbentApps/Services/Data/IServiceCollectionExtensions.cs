using ProbentApps.Model;

namespace ProbentApps.Services.Data;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services) => services
        .AddScoped<IAdvancementManager, AdvancementManager>()
        .AddScoped<IRepository<Advancement>, AdvancementManager>()
        .AddScoped<IReportManager, ReportManager>()
        .AddScoped<IRepository<Report>, ReportManager>()
        .AddScoped<IInvoicingManager, InvoicingManager>()
        .AddScoped<IRepository<Invoice>, InvoicingManager>()
        .ConfigureOptions<ApplicationStaticFilesOptionsConfiguration>();
}
