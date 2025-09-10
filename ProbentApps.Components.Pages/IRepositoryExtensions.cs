using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using MudBlazor;
using ProbentApps.Model;
using ProbentApps.Services.Data.Abstractions;
using ProbentApps.Services.Data.Abstractions.Querying;

namespace ProbentApps.Components;

internal static class IRepositoryExtensions
{
    public static Func<GridState<T>, Task<GridData<T>>> LoadTableDataFor<T>(this IRepository<T> repository, ClaimsPrincipal user, Func<IQueryable<T>, IQueryable<T>>? select = null)
        where T : class, IEntity =>
        async state =>
        {
            IQueryable<T> filter(IQueryable<T> query)
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
            }

            IQueryable<T> sortAndPaginate(IQueryable<T> query) => query
                .OrderBy(state.SortDefinitions)
                .Skip(state.PageSize * state.Page).Take(state.PageSize);

            GridData<T> data = new();

            (data.Items, data.TotalItems) = await repository.GetTableDataForAsync(new(filter, select, sortAndPaginate, user));

            return data;
        };
}
