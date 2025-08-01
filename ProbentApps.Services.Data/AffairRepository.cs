using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProbentApps.Model;
using ProbentApps.Services.Data.Abstractions;
using ProbentApps.Services.Database.Abstractions.Contexts;
using System.Security.Claims;

namespace ProbentApps.Services.Data;

public class AffairRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : DefaultRepository<Affair>(context), IAffairRepository
{
    private Func<ApplicationDbContext, Guid, Guid[], bool, IAsyncEnumerable<Affair>>? _getAffairsFor;
    private Func<ApplicationDbContext, Guid, Guid[], bool, IAsyncEnumerable<Affair>> GetAffairsFor => _getAffairsFor ??=
        EF.CompileAsyncQuery((ApplicationDbContext context, Guid userId, Guid[] extraAffairIds, bool archived) => context.Affairs
            .Where(a => a.IsArchived == archived
                && (extraAffairIds.Contains(a.Id) || context.Structures
                    .Where(s => s.Manager!.Id == userId)
                    .Any(s => a.Code.StartsWith(s.Code)))));

    private Func<ApplicationDbContext, Guid, IAsyncEnumerable<Affair>>? _getAllAffairsFrom;
    private Func<ApplicationDbContext, Guid, IAsyncEnumerable<Affair>> GetAllAffairsFrom => _getAllAffairsFrom ??=
        EF.CompileAsyncQuery((ApplicationDbContext context, Guid structureId) => context.Affairs
            .Where(a => a.Code.StartsWith(context.Structures.First(s => s.Id == structureId).Code)));

    IAsyncEnumerable<Affair> IAffairRepository.GetAffairsFor(ClaimsPrincipal user, bool archived) =>
        GetAffairsFor(Context, Guid.Parse(userManager.GetUserId(user)!), user.GetExtraManagedAffairs(), archived);

    IAsyncEnumerable<Affair> IAffairRepository.GetAllAffairsFrom(Guid structureId) =>
        GetAllAffairsFrom(Context, structureId);
}
