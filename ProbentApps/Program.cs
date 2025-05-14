using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MudBlazor.Services;
using ProbentApps.Components;
using ProbentApps.Database.Contexts;
using ProbentApps.Model;
using ProbentApps.Services;

var builder = WebApplication.CreateBuilder(args);

void configureDbContext<TContext>(DbContextOptionsBuilder options) where TContext : IDbContext =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(nameof(ProbentApps))
            ?? throw new InvalidOperationException(@$"{nameof(ProbentApps)}: No such connection string"),
            options => options.MigrationsHistoryTable("MigrationsHistory", TContext.Schema))
        .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
        .ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.SensitiveDataLoggingEnabledWarning))
#pragma warning disable ASP0000
        .UseInternalServiceProvider(new ServiceCollection().AddEntityFrameworkSqlServer()
            .AddConventionSetPlugins()
            .BuildServiceProvider());
#pragma warning restore ASP0000

builder.Services.AddDbContext<DataProtectionDbContext>(configureDbContext<DataProtectionDbContext>)
    .AddDbContext<IdentityDbContext, IdentityDbContext<IdentityDbFunctions>>(configureDbContext<IdentityDbContext>)
    .AddIdentityMigrationsDbContext(options =>
    {
        configureDbContext<IdentityDbContext>(options);
        options.UseAsyncSeeding(async (context, _, cancellationToken) =>
        {
        var users = context.Set<ApplicationUser>();
        if (!await users.AnyAsync(u => u.Id == ApplicationUser.RootId, cancellationToken))
            users.Add(new()
            {
                Id = ApplicationUser.RootId,
                UserName = "root",
                NormalizedUserName = "ROOT",
                Email = "root@apps.probent.local",
                NormalizedEmail = "ROOT@APPS.PROBENT.LOCAL",
                EmailConfirmed = true
            });
        if (!await users.AnyAsync(u => u.Id == Guid.AllBitsSet, cancellationToken))
            users.Add(new() { Id = Guid.AllBitsSet, UserName = "Deleted user" });
        await context.SaveChangesAsync(cancellationToken);
        });
    });

if (builder.Environment.IsDevelopment())
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDataProtection()
    .SetApplicationName(nameof(ProbentApps))
    .PersistKeysToDbContext<DataProtectionDbContext>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(static options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Stores.ProtectPersonalData = true;
        options.Stores.SchemaVersion = IdentitySchemaVersions.Version2;
        options.User.RequireUniqueEmail = true;
    })
    .AddPersonalDataProtection<LookupProtector, LookupProtectorKeyRing>()
    .AddEntityFrameworkStores<IdentityDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<ApplicationUser>, DataProtectionLookupProtectedUserManager<ApplicationUser>>();
builder.Services.AddScoped<IUserStore<ApplicationUser>, DataProtectionLookupProtectedUserStore<ApplicationUser, IdentityRole<Guid>, IdentityDbContext, Guid>>();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();
builder.Services.AddMudServices();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

var app = builder.Build();

if (!EF.IsDesignTime)
    await using (var scope = app.Services.CreateAsyncScope())
    {
        await scope.ServiceProvider.MigrateDatabaseAsync<DataProtectionDbContext>();
        await scope.ServiceProvider.MigrateDatabaseAsync<IdentityDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var rootUser = (await userManager.FindByIdAsync(ApplicationUser.RootId.ToString()))!;
        await userManager.RemovePasswordAsync(rootUser);
        await userManager.AddPasswordAsync(rootUser, "Test123!");
    }

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ProbentApps.Client._Imports).Assembly);

app.MapAdditionalIdentityEndpoints();

await app.RunAsync();
