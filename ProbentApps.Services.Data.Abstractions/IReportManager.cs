using ProbentApps.Model;

namespace ProbentApps.Services.Data.Abstractions;

public interface IReportManager : IRepository<Report>
{
    Task<ReportCreationResult> CreateReportAsync(ReportCreationData data, CancellationToken cancellationToken = default);

    Task<ReportUpdateResult> UpdateReportAsync(Guid reportId, ReportUpdateData data, CancellationToken cancellationToken = default);

    Task<AdvancementTrackingResult> TrackAdvancementInReportAsync(Guid reportId, Guid advancementId, CancellationToken cancellationToken = default);

    Task<AdvancementRemovalFromReportResult> RemoveAdvancementFromReportAsync(Guid reportId, Guid advancementId, CancellationToken cancellationToken = default);

    Task<ReportAcceptanceSubmissionResult> SubmitReportAcceptanceAsync(Guid reportId, DateOnly? acceptanceDate = null, CancellationToken cancellationToken = default);
}
