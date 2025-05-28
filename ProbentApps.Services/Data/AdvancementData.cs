using ProbentApps.Model;

namespace ProbentApps.Services.Data;

public record struct AdvancementCreationData(string Name, Guid OrderId, DateOnly? Date = null, string? Description = null);

public record struct AdvancementUpdateData(string? Name = null, DateOnly? Date = null, string? Description = null);

public record struct AdvancementCreationResult(AdvancementCreationResult.Status Result, Advancement? Entity = null)
{
    public enum Status : byte
    {
        Success,
        Cancelled,
        OrderNotFound,
        InvalidData
    }
}

public record struct AdvancementUpdateResult(AdvancementUpdateResult.Status Result)
{
    public enum Status : byte
    {
        Success,
        Cancelled,
        ReportAlreadyAccepted,
        InvoiceAlreadyRequested,
        InvalidData
    }
}

public record struct AdvancementDeletionResult(AdvancementDeletionResult.Status Result) 
{
    public enum Status : byte
    {
        Success,
        Cancelled,
        NotFound,
        InvoiceAlreadyRequested,
        ReportAlreadyAccepted
    }
}
