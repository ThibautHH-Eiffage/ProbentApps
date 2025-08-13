using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ProbentApps.Model;

public class Report : IEntity
{
    public Guid Id { get; set; }

    [MaxLength(64)]
    public string Name { get; set; } = default!;

    [MaxLength(128)]
    public string? Intermediaries { get; set; }

    [MaxLength(512)]
    public string? Notes { get; set; }

    public DateOnly IssuanceDate { get; set; }

    public DateOnly? AcceptanceDate { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Report? PreviousReport { get; set; }

    public IList<Advancement> Advancements { get; set; } = [];

    public IList<Order> Orders { get; set; } = [];

    public IList<Invoice> Invoices { get; set; } = [];
}
