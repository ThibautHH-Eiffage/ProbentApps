using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace ProbentApps.Services.Database.Abstractions.ValueGeneration;

internal static class PropertyBuilderExtensions
{
    public static PropertyBuilder HasComputedValue<TValueGenerator>(this PropertyBuilder builder, bool throwOnExplicitValue = false)
        where TValueGenerator : ValueGenerator
    {
        ArgumentNullException.ThrowIfNull(builder);

        var metadata = builder.Metadata;
        var propertySaveBehavior = throwOnExplicitValue ? PropertySaveBehavior.Throw : PropertySaveBehavior.Ignore;
        metadata.SetBeforeSaveBehavior(propertySaveBehavior);
        metadata.SetAfterSaveBehavior(propertySaveBehavior);

        return builder.HasValueGenerator<TValueGenerator>();
    }
}
