using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;

namespace ProbentApps.Services.Components;

internal static class IServiceCollectionExtensions
{
    public static IServiceCollection AddComponentServices(this IServiceCollection services)
    {
        var componentsBuilder = services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

        services.AddCascadingAuthenticationState();
        componentsBuilder.AddAuthenticationStateSerialization();

        services.AddScoped<IdentityUserAccessor>()
            .AddScoped<IdentityRedirectManager>();

        services.AddMudServices();

        return services;
    }
}
