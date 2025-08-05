using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProbentApps.Model;

[Table("Affairs")]
public class Affair : Structure, IEquatable<Affair>
{
    public bool IsArchived { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public required Client Client { get; set; }

    public required IList<Order> Orders { get; set; }

    public bool Equals(Affair? other) => other is not null
        && (Id.Equals(other.Id)
            || (Code?.Equals(other?.Code, StringComparison.Ordinal)
                ?? Name.Equals(other?.Name, StringComparison.CurrentCultureIgnoreCase)));

    public override bool Equals(object? obj) => Equals(obj as Affair);

    public override int GetHashCode() => Id.GetHashCode();

    public override string ToString() => Name;
}
