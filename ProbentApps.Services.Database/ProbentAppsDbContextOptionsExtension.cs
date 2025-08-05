using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace ProbentApps.Services.Database;

internal class ProbentAppsDbContextOptionsExtension : IDbContextOptionsExtension
{
    private sealed class ExtensionInfo(IDbContextOptionsExtension extension) : DbContextOptionsExtensionInfo(extension)
    {
        public override bool IsDatabaseProvider => false;

        public override string LogFragment => "NoDefaultSqlServerIdentityColumns UseSqlServerOptimizedUUIDv7UniqueIdentifiers ";

        public override int GetServiceProviderHashCode() => 0;

        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo) => debugInfo["ProbentApps:DisableDefaultSqlServerIdentityColumns"] = "true";

        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) => other is ExtensionInfo;
    }

    public static readonly ProbentAppsDbContextOptionsExtension Instance = new();

    private ProbentAppsDbContextOptionsExtension() { }

    private DbContextOptionsExtensionInfo? _info;
    public DbContextOptionsExtensionInfo Info => _info ??= new ExtensionInfo(this);

    public void ApplyServices(IServiceCollection services) => new EntityFrameworkServicesBuilder(services)
        .TryAdd<IConventionSetPlugin, ValueGenerationStrategyConventionRemoverConventionSetPlugin>()
        .TryAdd<IModelCustomizer, ProbentAppsModelCustomizer>();

    public void Validate(IDbContextOptions options) { }
}
