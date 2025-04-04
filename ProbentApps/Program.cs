using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using ProbentApps.Components;
using ProbentApps.Data;
using ProbentApps.Database.Contexts;
using ProbentApps.Services;
using IdentityDbContext = ProbentApps.Database.Contexts.IdentityDbContext;

var builder = WebApplication.CreateBuilder(args);

DbContextOptionsBuilder configureDbContext(DbContextOptionsBuilder options) =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(nameof(ProbentApps))
            ?? throw new InvalidOperationException(@$"{nameof(ProbentApps)}: No such connection string"))
        .EnableSensitiveDataLogging(builder.Environment.IsDevelopment());

builder.Services.AddDbContext<DataProtectionDbContext>(options => configureDbContext(options));
builder.Services.AddDbContextFactory<IdentityDbContext>(options => configureDbContext(options)
    .UseAsyncSeeding(async (context, _, cancellationToken) => {
        var users = context.Set<ApplicationUser>();
        if (await users.FindAsync([new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1)], cancellationToken) is null)
            users.Add(new() { Id = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1),
                UserName = "root", NormalizedUserName = "ROOT",
                Email = "root@probentapps", NormalizedEmail = "ROOT@PROBENTAPPS",
                EmailConfirmed = true });
        if (await users.FindAsync([Guid.AllBitsSet], cancellationToken) is null)
            users.Add(new() { Id = Guid.AllBitsSet, UserName = "Deleted user" });
        await context.SaveChangesAsync(cancellationToken);
    }));

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
        await scope.ServiceProvider.GetRequiredService<DataProtectionDbContext>().Database.MigrateAsync();
        await scope.ServiceProvider.GetRequiredService<IdentityDbContext>().Database.MigrateAsync();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var rootUser = (await userManager.FindByIdAsync(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1).ToString()))!;
        var zbui = await userManager.RemovePasswordAsync(rootUser);
        var iuzev=await userManager.AddPasswordAsync(rootUser, "Test123!");
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
