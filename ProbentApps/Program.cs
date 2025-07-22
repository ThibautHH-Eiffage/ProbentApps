using ProbentApps.Configuration;
using ProbentApps.Middleware;
using ProbentApps.Routes;
using ProbentApps.Services;
using ProbentApps.Services.Database;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
    builder.Configuration.AddDevelopmentConfiguration();
else
    builder.Configuration.AddProductionConfiguration();

builder.AddApplicationServices();

var app = builder.Build();

await app.MigrateDatabaseAsync();

if (app.Environment.IsDevelopment())
    app.UseDevelopmentMiddleware();
else
    app.UseProductionMiddleware();

app.UseApplicationMiddleware();

app.MapApplicationEndpoints();

await app.RunAsync();
