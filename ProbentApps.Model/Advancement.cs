using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ProbentApps.Model;

[Index($"{nameof(Order)}{nameof(Order.Id)}", $"{nameof(Report)}{nameof(Model.Report.Id)}", IsUnique = true)]
public class Advancement
{
    public Guid Id { get; set; }

    [MaxLength(64)]
    public required string Name { get; set; }

    [MaxLength(512)]
    public string? Description { get; set; }

    public DateOnly Date { get; set; }

    [Precision(38, 2)]
    public decimal Price { get; set; }

    public required Order Order { get; set; }

    public Report? Report { get; set; }

    public Invoice? Invoice { get; set; }
}
