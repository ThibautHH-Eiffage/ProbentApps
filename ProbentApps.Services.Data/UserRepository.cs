using Microsoft.EntityFrameworkCore;
using ProbentApps.Model;
using ProbentApps.Services.Data.Abstractions;
using ProbentApps.Services.Database.Abstractions.Contexts;

namespace ProbentApps.Services.Data;

internal class UserRepository(IDbContextFactory<ApplicationDbContext> contextFactory,
    IdentityDbContext identityDbContext)
	: DefaultRepository<ApplicationUser>(contextFactory), IUserRepository
{
    protected override IQueryable<ApplicationUser> ApplyDefaultDataSelection(IQueryable<ApplicationUser> query) => query
        .Select(static u => new ApplicationUser
        {
            Id = u.Id,
            UserName = u.UserName,
            Email = u.Email
        });

    Task<List<ApplicationUser>> IUserRepository.GetExtraManagersForAsync(Guid structureId, bool reverse, CancellationToken cancellationToken) =>
        ApplyDefaultDataSelection(identityDbContext.Users.AsNoTracking().Where(u =>
            u.Id != ApplicationUser.RootId
            && identityDbContext.UserClaims
                .Where(c => c.UserId == u.Id && c.ClaimType == ClaimsPrincipalExtensions.ExtraManagedStructuresClaimType)
                .Any(c => c.ClaimValue != null && (reverse ^ c.ClaimValue.Contains(structureId.ToString())))
            // TODO: Add the same condition for role claims
        ).OrderBy(static u => u.UserName).Take(10)).ToListAsync(cancellationToken);
}

