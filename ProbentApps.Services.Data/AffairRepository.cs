using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProbentApps.Model;
using ProbentApps.Services.Data.Abstractions;
using ProbentApps.Services.Data.Abstractions.Querying;
using ProbentApps.Services.Database.Abstractions.Contexts;

namespace ProbentApps.Services.Data;

internal class AffairRepository(IDbContextFactory<ApplicationDbContext> contextFactory, UserManager<ApplicationUser> userManager)
	: DefaultRepository<Affair>(contextFactory)
{
    protected override IQueryable<Affair> ApplyIdentityFilter(IQueryable<Affair> query, ClaimsPrincipal user) => query
        .WhereStructureIsAdministeredBy(Guid.Parse(userManager.GetUserId(user)!),
            user.GetExtraManagedStructures(),
            Context.Structures);

    protected override IQueryable<Affair> ApplyDefaultDataSelection(IQueryable<Affair> query) => query
        .Select(static a => new Affair
        {
            Id = a.Id,
            Name = a.Name,
            ShortCode = a.ShortCode,
            Client = new Client
            {
                Id = a.Client.Id,
                Name = a.Client.Name
            },
            IsArchived = a.IsArchived,
            Orders = a.Orders.Select(o => new Order
            {
                Id = o.Id,
                Name = o.Name,
                TotalPrice = o.TotalPrice,
            }).ToList()
        });
}

