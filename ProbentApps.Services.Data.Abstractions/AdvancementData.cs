using ProbentApps.Model;

namespace ProbentApps.Services.Data.Abstractions;

public record struct AdvancementCreationData(string Name, Guid OrderId, decimal Price, DateOnly? Date = null, string? Description = null);

public record struct AdvancementUpdateData(string? Name = null, decimal? Price = null, DateOnly? Date = null, string? Description = null);

public record struct AdvancementCreationResult(AdvancementCreationResult.Status Result, Advancement? Entity = null)
{
    public enum Status : byte
    {
        Success,
        OrderNotFound,
        InvalidData
    }
}

public record struct AdvancementUpdateResult(AdvancementUpdateResult.Status Result)
{
    public enum Status : byte
    {
        Success,
        NotFound,
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
        NotFound,
        InvoiceAlreadyRequested,
        ReportAlreadyAccepted
    }
}
