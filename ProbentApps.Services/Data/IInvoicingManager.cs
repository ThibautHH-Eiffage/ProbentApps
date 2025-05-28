using ProbentApps.Model;

namespace ProbentApps.Services.Data;

public interface IInvoicingManager : IRepository<Invoice>
{
    Task<InvoiceDraftingResult> DraftInvoiceAsync(InvoiceData invoice, CancellationToken cancellationToken = default);

    Task<InvoiceDraftModificationResult> ModifyInvoiceDraftAsync(Guid invoiceId, InvoiceData invoiceData, CancellationToken cancellationToken = default);

    Task<InvoiceDeletionResult> DeleteInvoiceAsync(Guid invoiceId, CancellationToken cancellationToken = default);

    Task<AdvancementInvoicingResult> InvoiceAdvancementsAsync(Guid invoiceId, IEnumerable<Guid> advancementIds, CancellationToken cancellationToken = default);

    Task<AdvancementRemovalFromInvoiceResult> RemoveAdvancementFromInvoiceAsync(Guid invoiceId, Guid advancementId, CancellationToken cancellationToken = default);

    Task<InvoiceRequestResult> RequestInvoiceAsync(Guid invoiceId, CancellationToken cancellationToken = default);

    Task<InvoiceSubmissionResult> SubmitInvoiceAsync(Guid invoiceId, string invoiceNumber, CancellationToken cancellationToken = default);
}
