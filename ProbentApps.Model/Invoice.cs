using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ProbentApps.Model;

public class Invoice : IEntity
{
    public Guid Id { get; set; }

    [MaxLength(64)]
    public required string Name { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public required ApplicationUser Requester { get; set; }

    public DateTimeOffset? RequestDate { get; set; }

    [MaxLength(64)]
    [Unicode(false)]
    public string? Code { get; set; }

    public DateTimeOffset? SubmissionDate { get; set; }

    public required IList<Advancement> Advancements { get; set; }

    public required IList<Order> Orders { get; set; }

    public required IList<Report> Reports { get; set; }
}
