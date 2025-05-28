using ProbentApps.Model;

namespace ProbentApps.Services.Data;

public record struct ReportCreationData(string Name, Guid? PreviousReportId,
        string? Intermediaries = null, string? Notes = null, DateOnly? IssuanceDate = null);

public record struct ReportUpdateData(string Name,
        string? Intermediaries = null, string? Notes = null, DateOnly? IssuanceDate = null);

public record struct ReportCreationResult(ReportCreationResult.Status Result, Report? Entity = null)
{
    public enum Status : byte
    {
        Success,
        Cancelled,
        PreviousReportNotFound,
        IssuancePredatesPreviousReportIssuance,
        InvalidData
    }
}

public record struct ReportUpdateResult(ReportUpdateResult.Status Result)
{
    public enum Status : byte
    {
        Success,
        Cancelled,
        NotFound,
        IssuancePredatesPreviousReportIssuance,
        AlreadyAccepted,
        InvalidData
    }
}

public record struct ReportDeletionResult(ReportDeletionResult.Status Result)
{
    public enum Status : byte
    {
        Success,
        Cancelled,
        NotFound,
        AlreadyAccepted
    }
}

public record struct AdvancementTrackingInReportResult(AdvancementTrackingInReportResult.Status Result)
{
    public enum Status : byte
    {
        Success,
        Cancelled,
        AdvancementNotFound,
        ReportNotFound,
        ReportAlreadyAccepted
    }
}

public record struct AdvancementRemovalFromReportResult(AdvancementRemovalFromReportResult.Status Result)
{
    public enum Status : byte
    {
        Success,
        Cancelled,
        AdvancementNotFound,
        AdvancementNotTrackedByReport,
        ReportNotFound,
        ReportAlreadyAccepted
    }
}

public record struct ReportAcceptanceSubmissionResult(ReportAcceptanceSubmissionResult.Status Result)
{
    public enum Status : byte
    {
        Success,
        Cancelled,
        NotFound,
        AcceptancePredatesIssuance,
        AlreadyAccepted
    }
}
