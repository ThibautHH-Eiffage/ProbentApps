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

internal class DefaultRepository<T>(IDbContextFactory<ApplicationDbContext> contextFactory) : IRepository<T>
    where T : class, IEntity
{
    private static readonly MethodInfo QueryParameterMarkerMethodInfo = typeof(EF).GetTypeInfo()
        .GetDeclaredMethod(nameof(EF.Parameter))
        !.MakeGenericMethod(typeof(Guid));

    private static readonly PropertyInfo EntityIdPropertyInfo = typeof(IEntity).GetTypeInfo()
        .GetDeclaredProperty(nameof(IEntity.Id))!;

    private AsyncLocal<ApplicationDbContext> _context = new();

    protected ApplicationDbContext Context => _context.Value!;

    protected IAsyncDisposable MakeQueryScope()
    {
        _context.Value = contextFactory.CreateDbContext();
        return Context;
    }

    private readonly struct QueryRoot(ApplicationDbContext context) : IQueryRoot
    {
        IQueryable<TEntity> IQueryRoot.GetEntitySet<TEntity>() => context.Set<TEntity>()
            .AsNoTrackingWithIdentityResolution();
    }

    protected virtual IQueryable<T> ApplyIdentityFilter(IQueryable<T> query, ClaimsPrincipal user) => query;

    protected virtual IQueryable<T> ApplyDefaultDataSelection(IQueryable<T> query) => query;

    async Task<IList<TResult>> IRepository<T>.Query<TResult>(QueryParameters<T, TResult> parameters, CancellationToken cancellationToken)
    {
        await using var scope = MakeQueryScope();

        var q = parameters.Filter(ApplyIdentityFilter(Context.Set<T>().AsNoTrackingWithIdentityResolution(), parameters.User));

        var query = parameters.SortAndPaginate((parameters.Select
            ?? throw new ArgumentException("Query parameters are missing a select expression", nameof(parameters)))
            (q, new QueryRoot(Context)));

        return (parameters.ToList
            ? await query.ToListAsync(cancellationToken)
            : await query.ToArrayAsync(cancellationToken));
    }

    async ValueTask<TResult> IRepository<T>.Query<TResult>(SingularQueryParameters<T, TResult> parameters, CancellationToken cancellationToken)
    {
        await using (var scope = MakeQueryScope())
            return await parameters.Select(parameters.Filter(ApplyIdentityFilter(
                    Context.Set<T>().AsNoTrackingWithIdentityResolution(), parameters.User)),
                    new QueryRoot(Context))
                .FirstAsync(cancellationToken);
    }

    async Task<(IList<T> data, int count)> IRepository<T>.GetTableDataForAsync(QueryParameters<T, T> parameters, CancellationToken cancellationToken)
    {
        await using var scope = MakeQueryScope();

        var query = parameters.Filter(ApplyIdentityFilter(Context.Set<T>().AsNoTrackingWithIdentityResolution(), parameters.User));

        query = parameters.Select is not null ? parameters.Select(query, new QueryRoot(Context)) : ApplyDefaultDataSelection(query);

        var count = await query.CountAsync(cancellationToken);

        query = parameters.SortAndPaginate(query);

        return (parameters.ToList ? await query.ToListAsync(cancellationToken) : await query.ToArrayAsync(cancellationToken), count);
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
