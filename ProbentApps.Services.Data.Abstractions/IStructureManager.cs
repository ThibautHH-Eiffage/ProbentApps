using ProbentApps.Model;

namespace ProbentApps.Services.Data.Abstractions;

public interface IStructureManager : IRepository<Structure>
{
    Task<bool> SetManagerAsync(Guid structureId, Guid? managerId, CancellationToken cancellationToken = default);

    Task<bool> SetStructureTypeAsync(Guid structureId, Guid structureTypeId, CancellationToken cancellationToken = default);
}
