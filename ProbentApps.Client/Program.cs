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

await builder.Build().RunAsync();
