using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;

namespace ProbentApps.Middleware;

public static class IApplicationBuilderExtensions
{
    public static void UseDevelopmentMiddleware(this IApplicationBuilder app) => app
        .UseMiddleware<MigrationsEndPointMiddleware>()
        .UseWebAssemblyDebugging();

    public static IApplicationBuilder UseProductionMiddleware(this IApplicationBuilder app) => app
        .UseExceptionHandler(Routes.Error.Endpoint, createScopeForErrors: true)
        .UseHsts();

    public static IApplicationBuilder UseApplicationMiddleware(this IApplicationBuilder app) => app
        .UseHttpsRedirection()
        .UseAuthentication()
        .UseAuthorization()
        .UseAntiforgery();
}
