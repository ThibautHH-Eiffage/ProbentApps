using System.Linq.Expressions;
using System.Security.Claims;
using ProbentApps.Model;
using ProbentApps.Services.Data.Abstractions.Querying;

namespace ProbentApps.Services.Data.Abstractions;

public interface IRepository<T>
    where T : class, IEntity
{
    Task<IList<TResult>> Query<TResult>(QueryParameters<T, TResult> parameters, CancellationToken cancellationToken = default);
    
    ValueTask<TResult> Query<TResult>(SingularQueryParameters<T, TResult> parameters, CancellationToken cancellationToken = default);

    Task<(IList<T> data, int count)> GetTableDataForAsync(QueryParameters<T, T> parameters, CancellationToken cancellationToken = default);

    IQueryable<T> ApplyEntityFilter(IQueryable<T> query, IEntity entity, LambdaExpression targetEntityExpression);
}
