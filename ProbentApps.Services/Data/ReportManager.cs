using Microsoft.EntityFrameworkCore;
using ProbentApps.Model;
using ProbentApps.Services.Data.Abstractions;
using ProbentApps.Services.Database.Abstractions.Contexts;

namespace ProbentApps.Services.Data;

public class ReportManager(ApplicationDbContext context, TimeProvider timeProvider) : DefaultRepository<Report>(context), IReportManager
{
    private Func<ApplicationDbContext, Guid, CancellationToken, Task<Report?>>? _getReport;
    private Func<ApplicationDbContext, Guid, CancellationToken, Task<Report?>> GetReport => _getReport ??=
        EF.CompileAsyncQuery((ApplicationDbContext context, Guid id, CancellationToken _) => context.Reports
            .Include(static r => r.PreviousReport)
            .Include(static r => r.Advancements)
            .FirstOrDefault(r => r.Id == id));

    public async Task<ReportCreationResult> CreateReportAsync(ReportCreationData data, CancellationToken cancellationToken = default)
    {
        Report? previousReport = null;
        
        if (data.PreviousReportId is Guid id)
        {
            previousReport = await Context.Reports.FindAsync([id], cancellationToken);
            if (previousReport is null)
            {
                return new ReportCreationResult(ReportCreationResult.Status.PreviousReportNotFound);
            }
        }

        var issuanceDate = data.IssuanceDate ?? DateOnly.FromDateTime(timeProvider.GetLocalNow().Date);

        if (issuanceDate < previousReport?.IssuanceDate)
        {
            return new ReportCreationResult(ReportCreationResult.Status.IssuancePredatesPreviousReportIssuance);
        }

        var report = Context.Add(new Report
        {
            Name = data.Name,
            PreviousReport = previousReport,
            IssuanceDate = issuanceDate,
            Intermediaries = data.Intermediaries,
            Notes = data.Notes,
            AcceptanceDate = null,
            Advancements = [],
            Orders = [],
            Invoices = [],
        }).Entity;

        try
        {
            await Context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return new ReportCreationResult(ReportCreationResult.Status.InvalidData);
        }

        return new ReportCreationResult(ReportCreationResult.Status.Success, report);
    }

    public async Task<ReportDeletionResult> DeleteReportAsync(Guid reportId, CancellationToken cancellationToken = default)
    {
        if (await Context.Reports.FindAsync([reportId], cancellationToken) is not Report report)
        {
            return new ReportDeletionResult(ReportDeletionResult.Status.NotFound);
        }

        if (report.AcceptanceDate is not null)
        {
            return new ReportDeletionResult(ReportDeletionResult.Status.AlreadyAccepted);
        }

        Context.Remove(report);
        await Context.SaveChangesAsync(cancellationToken);

        return new ReportDeletionResult(ReportDeletionResult.Status.Success);
    }

    public async Task<ReportUpdateResult> UpdateReportAsync(Guid reportId, ReportUpdateData data, CancellationToken cancellationToken = default)
    {
        if (await GetReport(Context, reportId, cancellationToken) is not Report report)
            return new ReportUpdateResult(ReportUpdateResult.Status.NotFound);

        if (report.AcceptanceDate is not null)
        {
            return new ReportUpdateResult(ReportUpdateResult.Status.AlreadyAccepted);
        }

        if (data.IssuanceDate < report.PreviousReport?.IssuanceDate)
        {
            return new ReportUpdateResult(ReportUpdateResult.Status.IssuancePredatesPreviousReportIssuance);
        }

        if (report.Advancements.Any(a => report.IssuanceDate < a.Date))
        {
            return new ReportUpdateResult(ReportUpdateResult.Status.IssuancePredatesAdavancement);
        }

        if (data.Name is string name)
            report.Name = name;
        if (data.Intermediaries is string intermediaries)
            report.Intermediaries = intermediaries;
        if (data.Notes is string notes)
            report.Notes = notes;
        if (data.IssuanceDate is DateOnly issuanceDate)
            report.IssuanceDate = issuanceDate;

        try
        {
            await Context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return new ReportUpdateResult(ReportUpdateResult.Status.InvalidData);
        }

        return new ReportUpdateResult(ReportUpdateResult.Status.Success);
    }

    public async Task<AdvancementTrackingResult> TrackAdvancementInReportAsync(Guid reportId, Guid advancementId, CancellationToken cancellationToken = default)
    {
        if (await Context.Reports.FindAsync([reportId], cancellationToken) is not Report report)
        {
            return new AdvancementTrackingResult(AdvancementTrackingResult.Status.ReportNotFound);
        }

        if (report.AcceptanceDate is not null)
        {
            return new AdvancementTrackingResult(AdvancementTrackingResult.Status.ReportAlreadyAccepted);
        }

        if (await Context.Advancements.FindAsync([advancementId], cancellationToken) is not Advancement advancement)
        {
            return new AdvancementTrackingResult(AdvancementTrackingResult.Status.AdvancementNotFound);
        }

        advancement.Report = report;

        await Context.SaveChangesAsync(cancellationToken);

        return new AdvancementTrackingResult(AdvancementTrackingResult.Status.Success);
    }

    public async Task<AdvancementRemovalFromReportResult> RemoveAdvancementFromReportAsync(Guid reportId, Guid advancementId, CancellationToken cancellationToken = default)
    {
        if (await Context.Reports.FindAsync([reportId], cancellationToken) is not Report report)
        {
            return new AdvancementRemovalFromReportResult(AdvancementRemovalFromReportResult.Status.ReportNotFound);
        }

        if (report.AcceptanceDate is not null)
        {
            return new AdvancementRemovalFromReportResult(AdvancementRemovalFromReportResult.Status.ReportAlreadyAccepted);
        }

        if (await Context.Advancements.FindAsync([advancementId], cancellationToken) is not Advancement advancement)
        {
            return new AdvancementRemovalFromReportResult(AdvancementRemovalFromReportResult.Status.AdvancementNotFound);
        }
        
        if (advancement.Report != report)
        {
            return new AdvancementRemovalFromReportResult(AdvancementRemovalFromReportResult.Status.AdvancementNotTrackedByReport);
        }

        advancement.Report = null;
        await Context.SaveChangesAsync(cancellationToken);

        return new AdvancementRemovalFromReportResult(AdvancementRemovalFromReportResult.Status.Success);
    }

    public async Task<ReportAcceptanceSubmissionResult> SubmitReportAcceptanceAsync(Guid reportId, DateOnly? acceptanceDate = null, CancellationToken cancellationToken = default)
    {
        if (await Context.Reports.FindAsync([reportId], cancellationToken) is not Report report)
        {
            return new ReportAcceptanceSubmissionResult(ReportAcceptanceSubmissionResult.Status.NotFound);
        }

        if (report.AcceptanceDate is not null)
        {
            return new ReportAcceptanceSubmissionResult(ReportAcceptanceSubmissionResult.Status.AlreadyAccepted);
        }

        acceptanceDate = acceptanceDate ?? DateOnly.FromDateTime(timeProvider.GetLocalNow().Date);

        if (acceptanceDate < report.IssuanceDate)
        {
            return new ReportAcceptanceSubmissionResult(ReportAcceptanceSubmissionResult.Status.AcceptancePredatesIssuance);
        }

        report.AcceptanceDate = acceptanceDate;
        await Context.SaveChangesAsync(cancellationToken);

        return new ReportAcceptanceSubmissionResult(ReportAcceptanceSubmissionResult.Status.Success);
    }
}
