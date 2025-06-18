using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ProbentApps.Model;

public class Report
{
    public Guid Id { get; set; }

    [MaxLength(64)]
    public required string Name { get; set; }

    [MaxLength(128)]
    public string? Intermediaries { get; set; }

    [MaxLength(512)]
    public string? Notes { get; set; }

    public DateOnly IssuanceDate { get; set; }

    public DateOnly? AcceptanceDate { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Report? PreviousReport { get; set; }

    public required IList<Advancement> Advancements { get; set; }

    public required IList<Order> Orders { get; set; }

    public required IList<Invoice> Invoices { get; set; }
}
