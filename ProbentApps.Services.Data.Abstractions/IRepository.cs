using ProbentApps.Model;

namespace ProbentApps.Services.Data.Abstractions;

public interface IRepository<T>
    where T : class, IEntity
{
    ValueTask<T?> FindAsync(Guid id, CancellationToken cancellationToken = default);
}
