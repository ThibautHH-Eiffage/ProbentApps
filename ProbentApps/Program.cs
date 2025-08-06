using ProbentApps.Configuration;
using ProbentApps.Middleware;
using ProbentApps.Routes;
using ProbentApps.Services;
using ProbentApps.Services.Database;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
    builder.Configuration.AddDevelopmentConfiguration();
else if (builder.Environment.IsStaging())
{
    builder.WebHost.UseStaticWebAssets();
    builder.Configuration.AddProductionConfiguration();
}
else
    builder.Configuration.AddProductionConfiguration();

builder.Services.AddApplicationServices(builder.Environment, builder.Configuration);

var app = builder.Build();

await app.MigrateDatabaseAsync();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
    app.UseDevelopmentMiddleware();
else if (app.Environment.IsStaging())
    app.UseStagingMiddleware();
else
    app.UseProductionMiddleware();

app.UseApplicationMiddleware();

app.MapApplicationEndpoints();

await app.RunAsync();
