using ProbentApps.Model;

namespace ProbentApps.Services.Data;

internal static class IQueryableExtensions
{
    public static IQueryable<T> WhereStructureIsAdministeredBy<T>(this IQueryable<T> query, IQueryable<Structure> structureSet, Guid userId, Guid[] extraStructureIds) where T : Structure =>
        query.Where(s => extraStructureIds.Contains(s.Id)
            || structureSet.Where(s => s.Manager!.Id == userId)
                .Any(structure => s.Code.StartsWith(structure.Code)));
}
