using Microsoft.EntityFrameworkCore;
using ProbentApps.Model;
using ProbentApps.Services.Data.Abstractions;
using ProbentApps.Services.Database.Abstractions.Contexts;

namespace ProbentApps.Services.Data;

public class InvoicingManager(ApplicationDbContext context, TimeProvider timeProvider) : DefaultRepository<Invoice>(context), IInvoicingManager
{
    private Func<ApplicationDbContext, Guid, CancellationToken, Task<Advancement?>>? _getAdvancementQuery;
    private Func<ApplicationDbContext, Guid, CancellationToken, Task<Advancement?>> GetAdvancement => _getAdvancementQuery ??=
        EF.CompileAsyncQuery((ApplicationDbContext context, Guid id, CancellationToken _) => context.Advancements
            .Include(static a => a.Invoice)
            .FirstOrDefault(a => a.Id == id));

    private Func<ApplicationDbContext, Guid, CancellationToken, Task<bool>>? _invoiceHasAdvancementsQuery;
    private Func<ApplicationDbContext, Guid, CancellationToken, Task<bool>> InvoiceHasAdvancements => _invoiceHasAdvancementsQuery ??=
        EF.CompileAsyncQuery((ApplicationDbContext context, Guid id, CancellationToken _) => context.Advancements
            .Any(a => a.Invoice != null && a.Invoice.Id == id));

    private Func<ApplicationDbContext, Guid, CancellationToken, Task<Invoice?>>? _getInvoiceWithOrdersQuery;
    private Func<ApplicationDbContext, Guid, CancellationToken, Task<Invoice?>> GetInvoiceWithOrders => _getInvoiceWithOrdersQuery ??=
        EF.CompileAsyncQuery((ApplicationDbContext context, Guid id, CancellationToken _) => context.Invoices
            .Include(static i => i.Orders).ThenInclude(static o => o.Client)
            .FirstOrDefault(i => i.Id == id));

    async Task<InvoiceCreationResult> IInvoicingManager.CreateInvoiceAsync(InvoiceCreationData data, CancellationToken cancellationToken)
    {
        var invoice = Context.Add(new Invoice
        {
            Name = data.Name,
            Requester = data.Requester,
            Advancements = [],
            Orders = [],
            Reports = []
        }).Entity;

        try
        {
            await Context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return new InvoiceCreationResult(InvoiceCreationResult.Status.InvalidData);
        }

        return new InvoiceCreationResult(InvoiceCreationResult.Status.Success, invoice);
    }

    async Task<InvoiceDeletionResult> IInvoicingManager.DeleteInvoiceAsync(Guid invoiceId, CancellationToken cancellationToken)
    {
        if (await Context.Invoices.FindAsync([invoiceId], cancellationToken) is not Invoice invoice)
        {
            return new InvoiceDeletionResult(InvoiceDeletionResult.Status.NotFound);
        }

        if (invoice.RequestDate is not null)
        {
            return new InvoiceDeletionResult(InvoiceDeletionResult.Status.AlreadyRequested);
        }

        Context.Remove(invoice);
        await Context.SaveChangesAsync(cancellationToken);

        return new InvoiceDeletionResult(InvoiceDeletionResult.Status.Success);
    }

    async Task<InvoiceUpdateResult> IInvoicingManager.UpdateInvoiceAsync(Guid invoiceId, InvoiceUpdateData data, CancellationToken cancellationToken)
    {
        if (await Context.Invoices.FindAsync([invoiceId], cancellationToken) is not Invoice invoice)
        {
            return new InvoiceUpdateResult(InvoiceUpdateResult.Status.NotFound);
        }

        if (invoice.RequestDate is not null)
        {
            return new InvoiceUpdateResult(InvoiceUpdateResult.Status.AlreadyRequested);
        }

        if (data.Name is string name)
            invoice.Name = name;

        try
        {
            await Context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return new InvoiceUpdateResult(InvoiceUpdateResult.Status.InvalidData);
        }

        return new InvoiceUpdateResult(InvoiceUpdateResult.Status.Success);
    }

    async Task<AdvancementInvoicingResult> IInvoicingManager.InvoiceAdvancementAsync(Guid invoiceId, Guid advancementId, CancellationToken cancellationToken)
    {
        if (await Context.Invoices.FindAsync([invoiceId], cancellationToken) is not Invoice invoice)
        {
            return new AdvancementInvoicingResult(AdvancementInvoicingResult.Status.InvoiceNotFound);
        }

        if (invoice.RequestDate is not null)
        {
            return new AdvancementInvoicingResult(AdvancementInvoicingResult.Status.InvoiceAlreadyRequested);
        }

        if (await GetAdvancement(Context, advancementId, cancellationToken) is not Advancement advancement)
        {
            return new AdvancementInvoicingResult(AdvancementInvoicingResult.Status.AdvancementNotFound);
        }

        if (advancement.Invoice is not null)
        {
            return new AdvancementInvoicingResult(AdvancementInvoicingResult.Status.AdvancementAlreadyInvoiced);
        }

        advancement.Invoice = invoice;

        await Context.SaveChangesAsync(cancellationToken);

        return new AdvancementInvoicingResult(AdvancementInvoicingResult.Status.Success);
    }

    async Task<AdvancementRemovalFromInvoiceResult> IInvoicingManager.RemoveAdvancementFromInvoiceAsync(Guid invoiceId, Guid advancementId, CancellationToken cancellationToken)
    {
        if (await Context.Invoices.FindAsync([invoiceId], cancellationToken) is not Invoice invoice)
        {
            return new AdvancementRemovalFromInvoiceResult(AdvancementRemovalFromInvoiceResult.Status.InvoiceNotFound);
        }

        if (invoice.RequestDate is not null)
        {
            return new AdvancementRemovalFromInvoiceResult(AdvancementRemovalFromInvoiceResult.Status.InvoiceAlreadyRequested);
        }

        if (await Context.Advancements.FindAsync([advancementId], cancellationToken) is not Advancement advancement)
        {
            return new AdvancementRemovalFromInvoiceResult(AdvancementRemovalFromInvoiceResult.Status.AdvancementNotFound);
        }
        
        if (advancement.Invoice != invoice)
        {
            return new AdvancementRemovalFromInvoiceResult(AdvancementRemovalFromInvoiceResult.Status.AdvancementNotInvoicedByInvoice);
        }

        advancement.Invoice = null;
        await Context.SaveChangesAsync(cancellationToken);

        return new AdvancementRemovalFromInvoiceResult(AdvancementRemovalFromInvoiceResult.Status.Success);
    }

    async Task<InvoiceRequestResult> IInvoicingManager.RequestInvoiceAsync(Guid invoiceId, CancellationToken cancellationToken)
    {
        if (await Context.Invoices.FindAsync([invoiceId], cancellationToken) is not Invoice invoice)
        {
            return new InvoiceRequestResult(InvoiceRequestResult.Status.NotFound);
        }

        if (invoice.RequestDate is not null)
        {
            return new InvoiceRequestResult(InvoiceRequestResult.Status.AlreadyRequested);
        }

        if (!await InvoiceHasAdvancements(Context, invoiceId, cancellationToken))
        {
            return new InvoiceRequestResult(InvoiceRequestResult.Status.EmptyInvoice);
        }

        invoice.RequestDate = timeProvider.GetLocalNow();
        await Context.SaveChangesAsync(cancellationToken);

        return new InvoiceRequestResult(InvoiceRequestResult.Status.Success);
    }

    async Task<InvoiceSubmissionResult> IInvoicingManager.SubmitInvoiceAsync(Guid invoiceId, string code, CancellationToken cancellationToken)
    {
        if (await GetInvoiceWithOrders(Context, invoiceId, cancellationToken) is not Invoice invoice)
        {
            return new InvoiceSubmissionResult(InvoiceSubmissionResult.Status.NotFound);
        }

        if (invoice.SubmissionDate is not null)
        {
            return new InvoiceSubmissionResult(InvoiceSubmissionResult.Status.AlreadySubmitted);
        }

        if (invoice.RequestDate is null)
        {
            return new InvoiceSubmissionResult(InvoiceSubmissionResult.Status.NotYetRequested);
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            return new InvoiceSubmissionResult(InvoiceSubmissionResult.Status.InvalidData);
        }

        if (invoice.Orders.Any(static o => o.Client.Code is null))
        {
            return new InvoiceSubmissionResult(InvoiceSubmissionResult.Status.UnregisteredClient);
        }

        invoice.SubmissionDate = timeProvider.GetLocalNow();
        invoice.Code = code;

        try 
        {
            await Context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return new InvoiceSubmissionResult(InvoiceSubmissionResult.Status.InvalidData);
        }

        return new InvoiceSubmissionResult(InvoiceSubmissionResult.Status.Success);
    }
}
