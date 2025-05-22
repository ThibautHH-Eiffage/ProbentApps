using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace ProbentApps.Database.Contexts;

public class ProbentAppsDbContextOptionsExtension : IDbContextOptionsExtension
{
    public static readonly ProbentAppsDbContextOptionsExtension Instance = new();

    private sealed class ExtensionInfo(IDbContextOptionsExtension extension) : DbContextOptionsExtensionInfo(extension)
    {
        public override bool IsDatabaseProvider => false;

        public override string LogFragment => "";

        public override int GetServiceProviderHashCode() => 0;

        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo) => debugInfo["ProbentApps:DisableSqlServerIdentityColumns"] = "true";

        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) => other is ExtensionInfo;
    }

    private DbContextOptionsExtensionInfo? _info;

    private ProbentAppsDbContextOptionsExtension() { }

    public DbContextOptionsExtensionInfo Info => _info ??= new ExtensionInfo(this);

    public void ApplyServices(IServiceCollection services) => new EntityFrameworkServicesBuilder(services)
        .TryAdd<IConventionSetPlugin, ValueGenerationStrategyConventionRemoverConventionSetPlugin>();

    public void Validate(IDbContextOptions options) { }
}
