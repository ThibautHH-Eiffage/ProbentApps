using ProbentApps.Model;
using ProbentApps.Services.Data.Abstractions;
using ProbentApps.Services.Database.Abstractions.Contexts;

namespace ProbentApps.Services.Data;

public class DefaultRepository<T>(ApplicationDbContext context) : IRepository<T>
    where T : class, IEntity
{
    protected ApplicationDbContext Context { get; } = context;

    ValueTask<T?> IRepository<T>.FindAsync(Guid id, CancellationToken cancellationToken) =>
        Context.Set<T>().FindAsync([id], cancellationToken);

    IQueryable<T> IRepository<T>.Query() => Context.Set<T>();
}
