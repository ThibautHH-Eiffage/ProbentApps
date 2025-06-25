using ProbentApps.Model;

namespace ProbentApps.Services.Data.Abstractions;

public interface IAdvancementManager : IRepository<Advancement>
{
    Task<AdvancementCreationResult> CreateAdvancementAsync(AdvancementCreationData data, CancellationToken cancellationToken = default);

    Task<AdvancementUpdateResult> UpdateAdvancementAsync(Guid advancementId, AdvancementUpdateData data, CancellationToken cancellationToken = default);

    Task<AdvancementDeletionResult> DeleteAdvancementAsync(Guid advancementId, CancellationToken cancellationToken = default);
}
