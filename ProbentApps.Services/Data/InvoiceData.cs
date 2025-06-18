using ProbentApps.Model;

namespace ProbentApps.Services.Data;

public record struct InvoiceCreationData(string Name, ApplicationUser Requester);

public record struct InvoiceUpdateData(string Name);

public record struct InvoiceCreationResult(InvoiceCreationResult.Status Result, Invoice? Entity = null)
{
    public enum Status : byte
    {
        Success,
        InvalidData
    }
}

public record struct InvoiceUpdateResult(InvoiceUpdateResult.Status Result)
{
    public enum Status : byte
    {
        Success,
        NotFound,
        AlreadyRequested,
        InvalidData
    }
}

public record struct AdvancementInvoicingResult(AdvancementInvoicingResult.Status Result)
{
    public enum Status : byte
    {
        Success,
        InvoiceNotFound,
        InvoiceAlreadyRequested,
        AdvancementNotFound,
        AdvancementAlreadyInvoiced
    }
}

public record struct AdvancementRemovalFromInvoiceResult(AdvancementRemovalFromInvoiceResult.Status Result)
{
    public enum Status : byte
    {
        Success,
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
        NotFound,
        AlreadyRequested
    }
}

public record struct InvoiceRequestResult(InvoiceRequestResult.Status Result)
{
    public enum Status : byte
    {
        Success,
        NotFound,
        EmptyInvoice,
        AlreadyRequested
    }
}

public record struct InvoiceSubmissionResult(InvoiceSubmissionResult.Status Result)
{
    public enum Status : byte
    {
        Success,
        NotFound,
        NotYetRequested,
        UnregisteredClient,
        AlreadySubmitted,
        InvalidData
    }
}
