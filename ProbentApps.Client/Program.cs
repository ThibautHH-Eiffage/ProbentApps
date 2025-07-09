using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using MudBlazor.Translations;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddLocalization()
    .AddMudServices()
    .AddMudTranslations();

builder.Services.AddAuthorizationCore()
    .AddCascadingAuthenticationState()
    .AddAuthenticationStateDeserialization();

// Workaround for WebAssemblyCultureProvider not loading UI culture
await LoadSatelliteAssemblies(["en", "fr"]);

await builder.Build().RunAsync();

internal static partial class Program
{
    [JSImport("INTERNAL.loadSatelliteAssemblies")]
    private static partial Task LoadSatelliteAssemblies(string[] culturesToLoad);
}
