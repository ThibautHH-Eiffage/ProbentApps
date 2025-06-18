using ProbentApps.Middleware;
using ProbentApps.Routes;
using ProbentApps.Services;
using ProbentApps.Services.Database;

var builder = WebApplication.CreateBuilder(args);

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
