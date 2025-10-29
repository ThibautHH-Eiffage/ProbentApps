using ProbentApps.Model;

namespace ProbentApps.Services.Data.Abstractions.Querying;

public interface IQueryRoot
{
    IQueryable<T> GetEntitySet<T>() where T : class, IEntity;
}
