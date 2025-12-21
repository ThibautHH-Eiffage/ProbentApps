using ProbentApps.Model;

namespace ProbentApps.Services.Data.Abstractions;

public interface IUserRepository : IRepository<ApplicationUser>
{
    Task<List<ApplicationUser>> GetExtraManagersForAsync(Guid structureId, bool reverse = false, CancellationToken cancellationToken = default);
}
