using System.Security.Claims;

namespace ProbentApps.Services.Data.Abstractions.Querying;

public record struct QueryParameters<T>(
    Func<IQueryable<T>, IQueryable<T>> Filter,
    Func<IQueryable<T>, IQueryable<T>>? Select,
    Func<IQueryable<T>, IQueryable<T>> SortAndPaginate,
    ClaimsPrincipal User)
    where T : class;
