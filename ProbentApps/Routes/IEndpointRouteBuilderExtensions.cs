using ProbentApps.Routes.Identity;

namespace ProbentApps.Routes;

internal static class IEndpointRouteBuilderExtensions
{
    public static IEndpointConventionBuilder MapApplicationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapStaticAssets();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(Components.Pages.Routes).Assembly, typeof(Components.Account._Imports).Assembly);

        return app.MapAdditionalIdentityEndpoints();
    }
}
