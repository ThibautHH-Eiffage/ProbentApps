using Microsoft.EntityFrameworkCore;
using ProbentApps.Database.Contexts;
using ProbentApps.Model;

namespace ProbentApps.Services.Data;

public abstract class RepositoryBase<T>(ApplicationDbContext context) : IRepository<T>
    where T : class, IEntity
{
    private Func<ApplicationDbContext, Guid, IAsyncEnumerable<T>>? _findQuery;
    private Func<ApplicationDbContext, Guid, IAsyncEnumerable<T>> Find => _findQuery ??=
        EF.CompileAsyncQuery((ApplicationDbContext context, Guid id) => context.Set<T>().Where(e => e.Id == id));

    public ValueTask<T?> FindAsync(Guid id, CancellationToken cancellationToken = default) =>
        Find(context, id).SingleOrDefaultAsync(cancellationToken);
}
