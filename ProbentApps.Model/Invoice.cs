using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ProbentApps.Model;

public class Invoice : IEntity
{
    public Guid Id { get; set; }

    [MaxLength(64)]
    public string Name { get; set; } = default!;

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public ApplicationUser Requester { get; set; } = default!;

    public DateTimeOffset? RequestDate { get; set; }

    [MaxLength(64)]
    [Unicode(false)]
    public string? Code { get; set; }

    public DateTimeOffset? SubmissionDate { get; set; }

    public IList<Advancement> Advancements { get; set; } = [];

    public IList<Order> Orders { get; set; } = [];

    public IList<Report> Reports { get; set; } = [];
}
