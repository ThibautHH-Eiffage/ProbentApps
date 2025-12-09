using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProbentApps.Model;
using ProbentApps.Services.Data.Abstractions;
using ProbentApps.Services.Database.Abstractions.Contexts;

namespace ProbentApps.Services.Data;

internal class StructureManager(IDbContextFactory<ApplicationDbContext> contextFactory,
    UserManager<ApplicationUser> userManager,
    TimeProvider timeProvider,
    ILogger<StructureManager> logger)
	: DefaultRepository<Structure>(contextFactory), IStructureManager
{
    protected override IQueryable<Structure> ApplyIdentityFilter(IQueryable<Structure> query, ClaimsPrincipal user) => query
        .WhereStructureIsAdministeredBy(
            Guid.Parse(userManager.GetUserId(user)!),
            user.GetExtraManagedStructures(),
            Context.Structures);

    async Task<bool> IStructureManager.SetManagerAsync(Guid structureId, Guid? managerId, CancellationToken cancellationToken)
    {
        await using var scope = MakeQueryScope(); // création d'un contexte pour éventuellement faire plusieurs query

        //récupération de l'objet structure associé à surtureId ou sortie (ne rien faire puisqh'il n'y a pas de structure)
        if (await Context.Structures.FindAsync([structureId], cancellationToken) is not Structure structure) return false;

        // définition de la date de modification (aujourd'hui)
        var currentDate = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);

        //Recherche du dernier historique connu (qui est celui du jour s'il à déjà été modifié)
        var lastStructureManagement = await Context.StructureManagements
            .Where(sm => sm.Structure.Id == structureId && sm.StartDate <= currentDate)
            .OrderByDescending(sm => sm.StartDate)
            .FirstOrDefaultAsync(cancellationToken);

        //Recherche du dernier historique avant ce jour 
        var previousStructureManagement = lastStructureManagement?.StartDate == currentDate
            ? await Context.StructureManagements
                .Where(sm => sm.Structure.Id == structureId && sm.StartDate < currentDate)
                .OrderByDescending(sm => sm.StartDate)
                .FirstOrDefaultAsync(cancellationToken)
               : lastStructureManagement;

        if (lastStructureManagement?.StartDate != currentDate)
        {
            if (managerId.Equals(previousStructureManagement?.Manager?.Id))
                logger.LogDebug("No change");
            else
            {
                logger.LogDebug("Add new");
                Context.Add(new StructureManagement
                {
                    Structure = structure,
                    StartDate = currentDate,
                    Manager = managerId is null ? null : Context.Attach(new ApplicationUser { Id = managerId.Value }).Entity // déclaration de l'objet new ApplicationUser comme déjà existant (avec Context.Attach) pour éviter la création en base de données
                });
            }
        }
        else
        {
            if (managerId.Equals(previousStructureManagement?.Manager?.Id))
            {
                logger.LogDebug("Delete current");
                Context.Remove(lastStructureManagement);
            }
            else
            {
                logger.LogDebug("Replace current");
                Context.StructureManagements.Entry(lastStructureManagement).Property<Guid?>("ManagerId").CurrentValue = managerId;
            }
        }

        // modification de la propriété ManagerId de l'entrée Structure chargée dans le context
        Context.Structures.Entry(structure).Property<Guid?>("ManagerId").CurrentValue = managerId;

        try
        {
            await Context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return false;
        }

        logger.LogDebug("Assign manager with ID: '{managerId}' to structure with ID: {structureId} ", managerId, structure.Id);

        return true;
    }

    async Task<bool> IStructureManager.SetStructureTypeAsync(Guid structureId, Guid structureTypeId, CancellationToken cancellationToken)
    {
        await using var scope = MakeQueryScope(); // création d'un contexte pour éventuellement faire plusieurs query

        //récupération de l'objet structure associé à surtureId ou sortie (ne rien faire puisqh'il n'y a pas de structure)
        if (await Context.Structures.FindAsync([structureId], cancellationToken) is not Structure structure) return false;

        // modification de la propriété ManagerId de l'entrée Structure chargée dans le context
        Context.Structures.Entry(structure).Property<Guid>("StructureTypeId").CurrentValue = structureTypeId;

        try
        {
            await Context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return false;
        }

        logger.LogDebug("Assign structure type with ID: '{structureTypeId}' to structure with ID: {structureId} ", structureTypeId, structure.Id);

        return true;
    }
}

