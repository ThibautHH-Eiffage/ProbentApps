using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using MudBlazor;
using ProbentApps.Model;
using ProbentApps.Services.Data.Abstractions;
using ProbentApps.Services.Data.Abstractions.Querying;

namespace ProbentApps.Components.Pages;

internal static class IRepositoryExtensions
{
    private static Func<IQueryable<T>, IQueryable<T>> MakeFilter<T>(this IRepository<T> repository, GridState<T> state)
        where T : class, IEntity => query =>
        {
            query = query.Where(state.FilterDefinitions);

            foreach ((var filterExpression, var entity) in state.FilterDefinitions
                .Where(static f => f.Value is IEntity)
                .Select(static f => (
                    (LambdaExpression)typeof(Column<T>).GetProperty("PropertyExpression", BindingFlags.NonPublic | BindingFlags.Instance)!
                        .GetValue(f.Column)!,
                    (IEntity)f.Value!)
                ))
                query = repository.ApplyEntityFilter(query, entity, filterExpression);

            return query;
        };
    
    public static Func<GridState<T>, QueryParameters<T, TResult>> MakeQueryParametersFor<T, TResult>(this IRepository<T> repository,
        ClaimsPrincipal user,
        Func<IQueryable<T>, IQueryRoot, IQueryable<TResult>>? select = null)
        where T : class, IEntity => state => new(repository.MakeFilter(state), select, static q => q, user);

    public static Func<GridState<T>, QueryParameters<T, TResult>> MakeQueryParametersFor<T, TResult>(this IRepository<T> repository,
        ClaimsPrincipal user,
        Func<IQueryable<T>, IQueryable<TResult>>? select = null)
        where T : class, IEntity => repository.MakeQueryParametersFor<T, TResult>(user, select is null ? null : (q, r) => select(q));

    public static Func<GridState<T>, Task<GridData<T>>> LoadTableDataFor<T>(this IRepository<T> repository, ClaimsPrincipal user,
        Func<IQueryable<T>, IQueryRoot, IQueryable<T>>? select = null,
        bool toList = false)
        where T : class, IEntity =>
        async state =>
        {
            GridData<T> data = new();

            (data.Items, data.TotalItems) = await repository.GetTableDataForAsync(
                repository.MakeQueryParametersFor(user, select)(state) with
                {
                    SortAndPaginate = query => query
                        .OrderBy(state.SortDefinitions)
                        .Skip(state.PageSize * state.Page).Take(state.PageSize),
                    ToList = toList
                });

            return data;
        };

    public static Func<GridState<T>, Task<GridData<T>>> LoadTableDataFor<T>(this IRepository<T> repository,
        ClaimsPrincipal user,
        Func<IQueryable<T>, IQueryable<T>>? select = null,
        bool toList = false)
        where T : class, IEntity => repository.LoadTableDataFor(user, select is null ? null : (q, r) => select(q), toList);
}
