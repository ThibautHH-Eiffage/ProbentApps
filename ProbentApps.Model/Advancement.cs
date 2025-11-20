using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ProbentApps.Model;

public class Advancement : IEntity, IEquatable<Advancement>
{
    public Guid Id { get; set; }

    [MaxLength(64)]
    public string Name { get; set; } = default!;

    [MaxLength(512)]
    public string? Description { get; set; }

    public DateOnly Date { get; set; }

    [Precision(38, 2)]
    public decimal Value { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Order Order { get; set; } = default!;

    [DeleteBehavior(DeleteBehavior.SetNull)]
    public Report? Report { get; set; }

    [DeleteBehavior(DeleteBehavior.SetNull)]
    public Invoice? Invoice { get; set; }

    public bool Equals(Advancement? other) => other is not null && Id.Equals(other.Id);

    public override bool Equals(object? obj) => Equals(obj as Advancement);

    public override int GetHashCode() => Id.GetHashCode();

    public override string ToString() => Name;
}
