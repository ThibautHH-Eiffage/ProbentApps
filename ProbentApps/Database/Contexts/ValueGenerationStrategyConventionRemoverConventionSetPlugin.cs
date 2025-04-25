using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace ProbentApps.Database.Contexts;

public class ValueGenerationStrategyConventionRemoverConventionSetPlugin : IConventionSetPlugin
{
    public ConventionSet ModifyConventions(ConventionSet conventionSet)
    {
        conventionSet.Remove(typeof(SqlServerValueGenerationStrategyConvention));
        return conventionSet;
    }
}
