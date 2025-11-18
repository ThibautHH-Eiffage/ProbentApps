using System.Security.Claims;

namespace ProbentApps.Services.Data.Abstractions.Querying;

public record struct QueryParameters<T, TResult>(
    Func<IQueryable<T>, IQueryable<T>> Filter,
    Func<IQueryable<T>, IQueryRoot, IQueryable<TResult>>? Select,
    Func<IQueryable<TResult>, IQueryable<TResult>> SortAndPaginate,
    ClaimsPrincipal User,
    bool ToList = false)
    where T : class;

public record struct SingularQueryParameters<T, TResult>(
    Func<IQueryable<T>, IQueryable<T>> Filter,
    Func<IQueryable<T>, IQueryRoot, IQueryable<TResult>> Select,
    ClaimsPrincipal User)
    where T : class;
