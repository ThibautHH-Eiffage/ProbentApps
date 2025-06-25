using ProbentApps.Model;

namespace ProbentApps.Services.Data.Abstractions;

public interface IInvoicingManager : IRepository<Invoice>
{
    Task<InvoiceCreationResult> CreateInvoiceAsync(InvoiceCreationData invoice, CancellationToken cancellationToken = default);

    Task<InvoiceUpdateResult> UpdateInvoiceAsync(Guid invoiceId, InvoiceUpdateData invoiceData, CancellationToken cancellationToken = default);

    Task<InvoiceDeletionResult> DeleteInvoiceAsync(Guid invoiceId, CancellationToken cancellationToken = default);

    Task<AdvancementInvoicingResult> InvoiceAdvancementAsync(Guid invoiceId, Guid advancementId, CancellationToken cancellationToken = default);

    Task<AdvancementRemovalFromInvoiceResult> RemoveAdvancementFromInvoiceAsync(Guid invoiceId, Guid advancementId, CancellationToken cancellationToken = default);

    Task<InvoiceRequestResult> RequestInvoiceAsync(Guid invoiceId, CancellationToken cancellationToken = default);

    Task<InvoiceSubmissionResult> SubmitInvoiceAsync(Guid invoiceId, string providerId, CancellationToken cancellationToken = default);
}
