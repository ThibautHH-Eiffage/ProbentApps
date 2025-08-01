using Microsoft.EntityFrameworkCore;
using ProbentApps.Services.Data.Abstractions;
using ProbentApps.Services.Database.Abstractions.Contexts;
using ProbentApps.Model;

namespace ProbentApps.Services.Data;

public class AdvancementManager(ApplicationDbContext context, TimeProvider timeProvider) : DefaultRepository<Advancement>(context), IAdvancementManager
{
    private Func<ApplicationDbContext, Guid, CancellationToken, Task<Advancement?>>? _getAdvancement;
    private Func<ApplicationDbContext, Guid, CancellationToken, Task<Advancement?>> GetAdvancement => _getAdvancement ??=
        EF.CompileAsyncQuery((ApplicationDbContext context, Guid id, CancellationToken _) => context.Advancements
            .Include(static a => a.Report)
            .Include(static a => a.Invoice)
            .FirstOrDefault(a => a.Id == id));

    async Task<AdvancementCreationResult> IAdvancementManager.CreateAdvancementAsync(AdvancementCreationData data, CancellationToken cancellationToken)
    {
        if (await Context.Orders.FindAsync([data.OrderId], cancellationToken) is not Order order)
        {
            return new AdvancementCreationResult(AdvancementCreationResult.Status.OrderNotFound);
        }

        Context.Attach(order);

        var advancement = Context.Add(new Advancement
        {
            Name = data.Name,
            Description = data.Description,
            Date = data.Date ?? DateOnly.FromDateTime(timeProvider.GetLocalNow().Date),
            Value = data.Price,
            Order = order
        }).Entity;

        try
        {
            await Context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return new AdvancementCreationResult(AdvancementCreationResult.Status.InvalidData);
        }

        return new AdvancementCreationResult(AdvancementCreationResult.Status.Success, advancement);
    }

    async Task<AdvancementDeletionResult> IAdvancementManager.DeleteAdvancementAsync(Guid advancementId, CancellationToken cancellationToken)
    {
        if (await GetAdvancement(Context, advancementId, cancellationToken) is not Advancement advancement)
            return new AdvancementDeletionResult(AdvancementDeletionResult.Status.NotFound);

        if (advancement.Report?.AcceptanceDate is not null)
        {
            return new AdvancementDeletionResult(AdvancementDeletionResult.Status.ReportAlreadyAccepted);
        }

        if (advancement.Invoice?.RequestDate is not null)
        {
            return new AdvancementDeletionResult(AdvancementDeletionResult.Status.InvoiceAlreadyRequested);
        }

        Context.Remove(advancement);
        await Context.SaveChangesAsync(cancellationToken);

        return new AdvancementDeletionResult(AdvancementDeletionResult.Status.Success);
    }

    async Task<AdvancementUpdateResult> IAdvancementManager.UpdateAdvancementAsync(Guid advancementId, AdvancementUpdateData data, CancellationToken cancellationToken)
    {
        if (await GetAdvancement(Context, advancementId, cancellationToken) is not Advancement advancement)
            return new AdvancementUpdateResult(AdvancementUpdateResult.Status.NotFound);

        if (advancement.Report?.AcceptanceDate is not null)
        {
            return new AdvancementUpdateResult(AdvancementUpdateResult.Status.ReportAlreadyAccepted);
        }

        if (advancement.Invoice?.RequestDate is not null)
        {
            return new AdvancementUpdateResult(AdvancementUpdateResult.Status.InvoiceAlreadyRequested);
        }

        if (data.Name is string name)
            advancement.Name = name;
        if (data.Date is DateOnly date)
            advancement.Date = date;
        if (data.Description is string description)
            advancement.Description = description;
        if (data.Price is decimal price)
            advancement.Value = price;

        try
        {
            await Context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return new AdvancementUpdateResult(AdvancementUpdateResult.Status.InvalidData);
        }

        return new AdvancementUpdateResult(AdvancementUpdateResult.Status.Success);
    }
}
