using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace ProbentApps.Services.Database;

internal sealed class ValueGenerationStrategyConventionRemoverConventionSetPlugin : IConventionSetPlugin
{
    public ConventionSet ModifyConventions(ConventionSet conventionSet)
    {
        conventionSet.Remove(typeof(SqlServerValueGenerationStrategyConvention));
        return conventionSet;
    }
}
