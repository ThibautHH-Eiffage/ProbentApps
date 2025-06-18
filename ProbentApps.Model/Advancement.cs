using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ProbentApps.Model;

public class Advancement : IEntity
{
    public Guid Id { get; set; }

    [MaxLength(64)]
    public required string Name { get; set; }

    [MaxLength(512)]
    public string? Description { get; set; }

    public DateOnly Date { get; set; }

    [Precision(38, 2)]
    public decimal Price { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public required Order Order { get; set; }

    [DeleteBehavior(DeleteBehavior.SetNull)]
    public Report? Report { get; set; }

    [DeleteBehavior(DeleteBehavior.SetNull)]
    public Invoice? Invoice { get; set; }
}
