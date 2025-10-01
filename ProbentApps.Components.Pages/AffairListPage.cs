using Microsoft.AspNetCore.Components;
using MudBlazor;
using ProbentApps.Model;
using ProbentApps.Services.Data.Abstractions;

namespace ProbentApps.Components.Pages;

public abstract class AffairListPage : AuthenticatedPage
{
    private IQueryable<Affair> SelectAffairListData(IQueryable<Affair> query) => query
        .Where(a => a.IsArchived == ArchivedOnly)
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
            Manager = a.Manager == null ? null : new ApplicationUser
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
                    Invoice = a.Invoice == null ? null : new Invoice { Id = a.Invoice!.Id }
                }).ToList()
            }).ToList()
        });

    private IQueryable<Client> SelectFilterClients(IQueryable<Affair> query) => query
        .Where(a => a.IsArchived == ArchivedOnly)
        .Select(static a => new Client
        {
            Id = a.Client.Id,
            Name = a.Client.Name
        });

    protected Func<GridState<Affair>, Task<GridData<Affair>>> TableDataLoader { get; private set; } = default!;

    protected Func<GridState<Affair>, Task<Client[]>> FilterClientsLoader { get; private set; } = default!;

    protected abstract bool ArchivedOnly { get; }

    [Inject]
    private IRepository<Affair> AffairRepository { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        TableDataLoader = AffairRepository.LoadTableDataFor(User, SelectAffairListData);

        FilterClientsLoader = state => AffairRepository.Query<Client>(AffairRepository.MakeQueryParametersFor(User, SelectFilterClients)(state));
    }
}
