using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProbentApps.Model;
using ProbentApps.Services.Data.Abstractions;
using ProbentApps.Services.Data.Abstractions.Querying;
using ProbentApps.Services.Database.Abstractions.Contexts;

namespace ProbentApps.Services.Data;

public class StructureRepository(IDbContextFactory<ApplicationDbContext> contextFactory, UserManager<ApplicationUser> userManager)
	: DefaultRepository<Structure>(contextFactory)
{
    protected override IQueryable<Structure> ApplyIdentityFilter(IQueryable<Structure> query, ClaimsPrincipal user) => query
        .WhereStructureIsAdministeredBy(Context.Structures,
            Guid.Parse(userManager.GetUserId(user)!),
            user.GetExtraManagedStructures());
}

