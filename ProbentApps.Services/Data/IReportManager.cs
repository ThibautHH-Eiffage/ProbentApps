using ProbentApps.Model;

namespace ProbentApps.Services.Data;

public interface IReportManager : IRepository<Report>
{
    Task<ReportCreationResult> CreateReportAsync(ReportCreationData data, CancellationToken cancellationToken = default);

    Task<ReportUpdateResult> UpdateReportAsync(Guid reportId, ReportUpdateData data, CancellationToken cancellationToken = default);

    Task<AdvancementTrackingInReportResult> TrackAdvancementsInReportAsync(Guid reportId, IEnumerable<Guid> advancementIds, CancellationToken cancellationToken = default);

    Task<AdvancementRemovalFromReportResult> RemoveAdvancementFromReportAsync(Guid reportId, Guid advancementId, CancellationToken cancellationToken = default);

    Task<ReportAcceptanceSubmissionResult> SubmitReportAcceptanceAsync(Guid reportId, DateOnly? acceptanceDate = null, CancellationToken cancellationToken = default);
}
