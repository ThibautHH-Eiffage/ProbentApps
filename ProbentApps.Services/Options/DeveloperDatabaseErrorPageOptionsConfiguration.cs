using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace ProbentApps.Services.Options;

internal class DeveloperDatabaseErrorPageConfiguration
    : IConfigureOptions<DatabaseErrorPageOptions>,
    IConfigureOptions<MigrationsEndPointOptions>
{
    private const string MigrationsEndPointPath = "/api/migrate";

    public void Configure(MigrationsEndPointOptions options)
    {
        options.Path = MigrationsEndPointPath;
    }

    public void Configure(DatabaseErrorPageOptions options)
    {
        options.MigrationsEndPointPath = MigrationsEndPointPath;
    }
}
