using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ProbentApps.Services.Database;

internal class ProbentAppsModelCustomizer(ModelCustomizerDependencies dependencies) : ModelCustomizer(dependencies)
{
    public override void Customize(ModelBuilder modelBuilder, DbContext context)
    {
        base.Customize(modelBuilder, context);

        foreach (IMutableProperty keyProperty in modelBuilder.Model.GetEntityTypes()
                .Select(static et => et.FindPrimaryKey())
                .Where(static pk => pk?.Properties.Count == 1)
                .Select(static pk => pk!.Properties[0])
                .Where(static p => p.ClrType == typeof(Guid) && p.ValueGenerated.HasFlag(ValueGenerated.OnAdd)))
                keyProperty.SetValueGeneratorFactory(static (_, _) => new UUIDv7GuidValueGenerator());
    }
}
