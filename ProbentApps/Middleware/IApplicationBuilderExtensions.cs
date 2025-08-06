namespace ProbentApps.Middleware;

public static class IApplicationBuilderExtensions
{
    public static void UseDevelopmentMiddleware(this IApplicationBuilder app) => app
        .UseDeveloperExceptionPage()
        .UseWebAssemblyDebugging();

    public static IApplicationBuilder UseProductionMiddleware(this IApplicationBuilder app) => app
        .UseExceptionHandler(Routes.Error.Endpoint, createScopeForErrors: true)
        .UseHsts();

    public static IApplicationBuilder UseApplicationMiddleware(this IApplicationBuilder app) => app
        .UseForwardedHeaders()
        .UseAuthentication()
        .UseAuthorization()
        .UseAntiforgery()
        .UseRequestLocalization("en", "fr");
}
