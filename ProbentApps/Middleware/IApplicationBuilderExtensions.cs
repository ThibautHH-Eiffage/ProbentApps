namespace ProbentApps.Middleware;

public static class IApplicationBuilderExtensions
{
    public static void UseDevelopmentMiddleware(this IApplicationBuilder app) => app
        .UseDeveloperExceptionPage()
        .UseWebAssemblyDebugging();

    public static IApplicationBuilder UseStagingMiddleware(this IApplicationBuilder app) => app
        .UseExceptionHandler(Routes.Error.Endpoint, createScopeForErrors: true);

    public static IApplicationBuilder UseProductionMiddleware(this IApplicationBuilder app) => app
        .UseExceptionHandler(Routes.Error.Endpoint, createScopeForErrors: true)
        .UseHsts();

    public static IApplicationBuilder UseApplicationMiddleware(this IApplicationBuilder app) => app
        .UseForwardedHeaders()
        .UseHttpLogging()
        .UseAuthentication()
        .UseAuthorization()
        .UseAntiforgery()
        .UseRequestLocalization("en", "fr");
}
