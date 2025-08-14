using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProbentApps.Model;
using ProbentApps.Services.Data.Abstractions;
using ProbentApps.Services.Database.Abstractions.Contexts;

namespace ProbentApps.Services.Data;

public class AffairRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : DefaultRepository<Affair>(context), IAffairRepository
{
    private Func<ApplicationDbContext, Guid, IAsyncEnumerable<Affair>>? _getAllAffairsFrom;
    private Func<ApplicationDbContext, Guid, IAsyncEnumerable<Affair>> GetAllAffairsFrom => _getAllAffairsFrom ??=
        EF.CompileAsyncQuery((ApplicationDbContext context, Guid structureId) => context.Affairs
            .Where(a => a.Code.StartsWith(context.Structures.First(s => s.Id == structureId).Code)));

    IAsyncEnumerable<Affair> IAffairRepository.GetAffairsFor(ClaimsPrincipal user, bool archived) => Context.Affairs
        .AsNoTrackingWithIdentityResolution()
        .Include(a => a.Client)
        .Include(a => a.Orders)
            .ThenInclude(o => o.Advancements)
                .ThenInclude(a => a.Invoice)
        .WhereStructureIsAdministeredBy(context.Structures, Guid.Parse(userManager.GetUserId(user)!), user.GetExtraManagedStructures())
        .Where(a => a.IsArchived == archived)
        .AsAsyncEnumerable();

    IAsyncEnumerable<Affair> IAffairRepository.GetAllAffairsFrom(Guid structureId) =>
        GetAllAffairsFrom(Context, structureId);
}

