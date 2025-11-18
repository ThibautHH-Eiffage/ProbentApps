using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using ProbentApps.Model;

namespace ProbentApps.Services.Data;

internal static class IQueryableExtensions
{
    private sealed class ReplacingExpressionVisitor(Expression target, Expression replacement)
        : ExpressionVisitor
    {
        [return: NotNullIfNotNull(nameof(e))]
        public override Expression? Visit(Expression? e) => e?.Equals(target) ?? false ? replacement : base.Visit(e);
    }

    private static Expression Replace(this Expression expression, Expression target, Expression replacement) =>
        new ReplacingExpressionVisitor(target, replacement).Visit(expression);

    public static IQueryable<T> WhereStructureIsAdministeredBy<T>(this IQueryable<T> query,
        Guid userId, Guid[] extraStructureIds,
        IQueryable<Structure> structureSet)
        where T : Structure => query
            .Where(structure => extraStructureIds.Contains(structure.Id)
                || structureSet.Where(s => s.Manager!.Id == userId)
                    .Any(s => structure.Code.StartsWith(s.Code)));

    public static IQueryable<T> WhereStructureIsAdministeredBy<T, TStructure>(this IQueryable<T> query,
        Guid userId, Guid[] extraStructureIds,
        IQueryable<Structure> structureSet,
        Expression<Func<T, TStructure>> structureSelector)
        where T : class, IEntity
        where TStructure : Structure
        {
            Expression<Func<TStructure, bool>> filter = structure =>
                extraStructureIds.Contains(structure.Id)
                    || structureSet.Where(s => s.Manager!.Id == userId)
                        .Any(s => structure.Code.StartsWith(s.Code));

            return query.Where(Expression.Lambda<Func<T, bool>>(
                filter.Body.Replace(filter.Parameters[0], structureSelector.Body),
                structureSelector.Parameters));
        }
}
