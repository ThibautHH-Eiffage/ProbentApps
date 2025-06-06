using ProbentApps.Components;
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
            .AddAdditionalAssemblies(typeof(ProbentApps.Client._Imports).Assembly);

        return app.MapAdditionalIdentityEndpoints();
    }
}
