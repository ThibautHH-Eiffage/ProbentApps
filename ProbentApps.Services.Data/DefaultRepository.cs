using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ProbentApps.Model;
using ProbentApps.Services.Data.Abstractions;
using ProbentApps.Services.Data.Abstractions.Querying;
using ProbentApps.Services.Database.Abstractions.Contexts;

namespace ProbentApps.Services.Data;

public class DefaultRepository<T>(ApplicationDbContext context) : IRepository<T>
    where T : class, IEntity
{
    private static readonly MethodInfo QueryParameterMarkerMethodInfo = typeof(EF).GetTypeInfo()
        .GetDeclaredMethod(nameof(EF.Parameter))
        !.MakeGenericMethod(typeof(Guid));

    private static readonly PropertyInfo EntityIdPropertyInfo = typeof(IEntity).GetTypeInfo()
        .GetDeclaredProperty(nameof(IEntity.Id))!;

    protected ApplicationDbContext Context { get; } = context;

    protected virtual IQueryable<T> ApplyIdentityFilter(IQueryable<T> query, ClaimsPrincipal user) => query;

    protected virtual IQueryable<T> ApplyDefaultDataSelection(IQueryable<T> query) => query;

    ValueTask<T?> IRepository<T>.FindAsync(Guid id, CancellationToken cancellationToken) =>
        Context.Set<T>().FindAsync([id], cancellationToken);

    async Task<(IEnumerable<T> data, int count)> IRepository<T>.GetTableDataForAsync(QueryParameters<T> parameters, CancellationToken cancellationToken)
    {
        var query = (parameters.Select ?? ApplyDefaultDataSelection)(parameters.Filter(ApplyIdentityFilter(Context.Set<T>().AsNoTrackingWithIdentityResolution(), parameters.User)));

        return (await parameters.SortAndPaginate(query).ToArrayAsync(cancellationToken), await query.CountAsync(cancellationToken));
    }

    IQueryable<T> IRepository<T>.ApplyEntityFilter(IQueryable<T> query, IEntity entity, LambdaExpression targetEntityExpression) => query
        .Where(Expression.Lambda<Func<T, bool>>(
            Expression.Equal(
                targetEntityExpression.Parameters.Single()
                    .MakeMemberAccess(targetEntityExpression.GetPropertyAccess())
                    .MakeMemberAccess(EntityIdPropertyInfo),
                Expression.Call(null, QueryParameterMarkerMethodInfo, Expression.Constant(entity.Id))
            ),
            targetEntityExpression.Parameters
        ));
}
