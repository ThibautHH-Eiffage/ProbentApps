using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProbentApps.Model;
using ProbentApps.Services.Data.Abstractions;
using ProbentApps.Services.Database.Abstractions.Contexts;

namespace ProbentApps.Services.Data;

public class AffairRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : DefaultRepository<Affair>(context), IAffairRepository
{
    IAsyncEnumerable<Affair> IAffairRepository.GetAffairsFor(ClaimsPrincipal user, bool archived) => Context.Affairs
        .AsNoTrackingWithIdentityResolution()
        .Include(a => a.Client)
        .Include(a => a.Orders)
            .ThenInclude(o => o.Advancements)
                .ThenInclude(a => a.Invoice)
        .WhereStructureIsAdministeredBy(context.Structures, Guid.Parse(userManager.GetUserId(user)!), user.GetExtraManagedStructures())
        .Where(a => a.IsArchived == archived)
        .Select(static a => new Affair
        {
            Id = a.Id,
            Name = a.Name,
            Code = a.Code,
            ShortCode = a.ShortCode,
            Client = new Client
            {
                Id = a.Client.Id,
                Name = a.Client.Name
            },
            IsArchived = a.IsArchived,
            Manager = EF.Property<Guid?>(a, "ManagerId") == null ? null : new ApplicationUser
            {
                Id = a.Manager!.Id,
                UserName = a.Manager.UserName,
                Email = a.Manager.Email
            },
            Orders = a.Orders.Select(o => new Order
            {
                Id = o.Id,
                Name = o.Name,
                TotalPrice = o.TotalPrice,
                Advancements = o.Advancements.Select(a => new Advancement
                {
                    Id = a.Id,
                    Value = a.Value,
                    Invoice = EF.Property<Guid?>(a, "InvoiceId") == null ? null : new Invoice { Id = a.Invoice!.Id }
                }).ToList()
            }).ToList()
        })
        .AsAsyncEnumerable();
}

