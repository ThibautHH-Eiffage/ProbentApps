using ProbentApps.Model;

namespace ProbentApps.Services.Data;

public record struct InvoiceData(string Name);

public record struct InvoiceDraftingResult(InvoiceDraftingResult.Status Result, Invoice? Entity = null)
{
    public enum Status : byte
    {
        Success,
        Cancelled,
        InvalidData
    }
}

public record struct InvoiceDraftModificationResult(InvoiceDraftModificationResult.Status Result)
{
    public enum Status : byte
    {
        Success,
        Cancelled,
        InvalidData
    }
}

public record struct AdvancementInvoicingResult(AdvancementInvoicingResult.Status Result)
{
    public enum Status : byte
    {
        Success,
        Cancelled,
        AdvancementNotFound,
        InvoiceNotFound,
        InvoiceAlreadyRequested
    }
}

public record struct AdvancementRemovalFromInvoiceResult(AdvancementRemovalFromInvoiceResult.Status Result)
{
    public enum Status : byte
    {
        Success,
        Cancelled,
        AdvancementNotFound,
        AdvancementNotInvoicedByInvoice,
        InvoiceNotFound,
        InvoiceAlreadyRequested
    }
}

public record struct InvoiceDeletionResult(InvoiceDeletionResult.Status Result)
{
    public enum Status : byte
    {
        Success,
        Cancelled,
        NotFound,
        AlreadyRequested
    }
}

public record struct InvoiceRequestResult(InvoiceRequestResult.Status Result)
{
    public enum Status : byte
    {
        Success,
        Cancelled,
        NotFound,
        InvoiceEmpty,
        AlreadyRequested
    }
}

public record struct InvoiceSubmissionResult(InvoiceSubmissionResult.Status Result)
{
    public enum Status : byte
    {
        Success,
        Cancelled,
        NotFound,
        NotYetRequested,
        UnregisteredClient,
        AlreadySubmitted,
        InvalidData
    }
}
