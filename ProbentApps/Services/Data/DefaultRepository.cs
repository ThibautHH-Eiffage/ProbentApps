using ProbentApps.Database.Contexts;
using ProbentApps.Model;

namespace ProbentApps.Services.Data;

public class DefaultRepository<T>(ApplicationDbContext context) : IRepository<T>
    where T : class, IEntity
{
    protected ApplicationDbContext Context { get; } = context;

    public ValueTask<T?> FindAsync(Guid id, CancellationToken cancellationToken = default) =>
        Context.Set<T>().FindAsync([id], cancellationToken);

    public IQueryable<T> Query() => Context.Set<T>();
}
