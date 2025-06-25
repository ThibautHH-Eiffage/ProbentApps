using ProbentApps.Model;

namespace ProbentApps.Services.Data.Abstractions;

public record struct ReportCreationData(string Name, Guid? PreviousReportId,
        string? Intermediaries = null, string? Notes = null, DateOnly? IssuanceDate = null);

public record struct ReportUpdateData(string Name,
        string? Intermediaries = null, string? Notes = null, DateOnly? IssuanceDate = null);

public record struct ReportCreationResult(ReportCreationResult.Status Result, Report? Entity = null)
{
    public enum Status : byte
    {
        Success,
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
        NotFound,
        IssuancePredatesPreviousReportIssuance,
        IssuancePredatesAdavancement,
        AlreadyAccepted,
        InvalidData
    }
}

public record struct ReportDeletionResult(ReportDeletionResult.Status Result)
{
    public enum Status : byte
    {
        Success,
        NotFound,
        AlreadyAccepted
    }
}

public record struct AdvancementTrackingResult(AdvancementTrackingResult.Status Result)
{
    public enum Status : byte
    {
        Success,
        ReportNotFound,
        ReportAlreadyAccepted,
        AdvancementNotFound,
        AdvancementAlreadyTracked
    }
}

public record struct AdvancementRemovalFromReportResult(AdvancementRemovalFromReportResult.Status Result)
{
    public enum Status : byte
    {
        Success,
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
        NotFound,
        AcceptancePredatesIssuance,
        AlreadyAccepted
    }
}
