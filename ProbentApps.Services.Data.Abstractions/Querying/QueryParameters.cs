using System.Security.Claims;

namespace ProbentApps.Services.Data.Abstractions.Querying;

public record struct QueryParameters<T, TResult>(
    Func<IQueryable<T>, IQueryable<T>> Filter,
    Func<IQueryable<T>, IQueryable<TResult>>? Select,
    Func<IQueryable<TResult>, IQueryable<TResult>> SortAndPaginate,
    ClaimsPrincipal User)
    where T : class
    where TResult : class;
