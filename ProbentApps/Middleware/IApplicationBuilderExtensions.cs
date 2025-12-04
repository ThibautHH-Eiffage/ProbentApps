namespace ProbentApps.Middleware;

public static class IApplicationBuilderExtensions
{
    public static void UseDevelopmentMiddleware(this IApplicationBuilder app) => app
        .UseDeveloperExceptionPage()
        .UseForwardedHeaders()
        .UseWebAssemblyDebugging();

    public static IApplicationBuilder UseStagingMiddleware(this IApplicationBuilder app) => app
        .UseExceptionHandler(Routes.Error.Endpoint, createScopeForErrors: true)
        .UseForwardedHeaders();

    public static IApplicationBuilder UseProductionMiddleware(this IApplicationBuilder app) => app
        .UseStagingMiddleware()
        .UseHsts();

    public static IApplicationBuilder UseApplicationMiddleware(this IApplicationBuilder app) => app
        .UseHttpLogging()
        .UseAuthentication()
        .UseAuthorization()
        .UseAntiforgery()
        .UseRequestLocalization("en", "fr", "fr-FR");
}
