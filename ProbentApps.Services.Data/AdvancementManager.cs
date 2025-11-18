using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProbentApps.Model;
using ProbentApps.Services.Data.Abstractions;
using ProbentApps.Services.Database.Abstractions.Contexts;

namespace ProbentApps.Services.Data;

internal class AdvancementManager(IDbContextFactory<ApplicationDbContext> contextFactory,
    UserManager<ApplicationUser> userManager,
    TimeProvider timeProvider,
    ILogger<AdvancementManager> logger)
    : DefaultRepository<Advancement>(contextFactory), IAdvancementManager
{
    protected override IQueryable<Advancement> ApplyIdentityFilter(IQueryable<Advancement> query, ClaimsPrincipal user) => query
        .WhereStructureIsAdministeredBy(Guid.Parse(userManager.GetUserId(user)!),
            user.GetExtraManagedStructures(),
            Context.Structures,
            static a => a.Order.Affair);

    protected override IQueryable<Advancement> ApplyDefaultDataSelection(IQueryable<Advancement> query) => query
        .Include(static a => a.Order)
        .ThenInclude(static o => o.Advancements)
        .Include(static a => a.Report)
        .Include(static a => a.Invoice);

    async Task<AdvancementCreationResult> IAdvancementManager.CreateAdvancementAsync(AdvancementCreationData data, CancellationToken cancellationToken)
    {
        await using var scope = MakeQueryScope();

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

        logger.LogDebug("Created advancement with ID: {Id} on order with ID: {OrderId}", advancement.Id, advancement.Order.Id);

        return new AdvancementCreationResult(AdvancementCreationResult.Status.Success, advancement);
    }

    async Task<AdvancementDeletionResult> IAdvancementManager.DeleteAdvancementAsync(Guid advancementId, CancellationToken cancellationToken)
    {
        await using var scope = MakeQueryScope();

        if (await Context.Advancements.Select(static a => new Advancement
                {
                    Id = a.Id,
                    Report = a.Report == null ? null : new Report { AcceptanceDate = a.Report.AcceptanceDate },
                    Invoice = a.Invoice == null ? null : new Invoice { RequestDate = a.Invoice.RequestDate }
                })
                .FirstOrDefaultAsync(a => a.Id == advancementId, cancellationToken) is not Advancement advancement)
        {
            return new AdvancementDeletionResult(AdvancementDeletionResult.Status.NotFound);
        }

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

        logger.LogDebug("Deleted advancement with ID: {Id}", advancement.Id);

        return new AdvancementDeletionResult(AdvancementDeletionResult.Status.Success);
    }

    async Task<AdvancementUpdateResult> IAdvancementManager.UpdateAdvancementAsync(Guid advancementId, AdvancementUpdateData data, CancellationToken cancellationToken)
    {
        await using var scope = MakeQueryScope();

        if (await Context.Advancements.Select(static a => new Advancement
                {
                    Id = a.Id,
                    Name = a.Name,
                    Date = a.Date,
                    Description = a.Description,
                    Value = a.Value,
                    Report = a.Report == null ? null : new Report { AcceptanceDate = a.Report.AcceptanceDate },
                    Invoice = a.Invoice == null ? null : new Invoice { RequestDate = a.Invoice.RequestDate }
                })
                .FirstOrDefaultAsync(a => a.Id == advancementId, cancellationToken) is not Advancement advancement)
        {
            return new AdvancementUpdateResult(AdvancementUpdateResult.Status.NotFound);
        }

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

        logger.LogDebug("Created advancement with ID: {Id}", advancement.Id);

        return new AdvancementUpdateResult(AdvancementUpdateResult.Status.Success);
    }
}
