using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using MudBlazor.Translations;

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

        services.AddLocalization()
            .AddMudServices()
            .AddMudTranslations();

        return services;
    }
}
