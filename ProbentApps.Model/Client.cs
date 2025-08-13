using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ProbentApps.Model;

public class Client : IEquatable<Client>
{
    public Guid Id { get; set; }

    [MaxLength(128)]
    public string Name { get; set; } = default!;

    [MaxLength(64)]
    [Unicode(false)]
    public string? Code { get; set; }

    public string[] ExtraCodes { get; set; } = [];

    public bool Equals(Client? other) => other is not null
        && (Id.Equals(other.Id)
            || ((Id.Equals(default) || other.Id.Equals(default))
            && (Code?.Equals(other.Code, StringComparison.Ordinal)
                ?? Name.Equals(other.Name, StringComparison.CurrentCultureIgnoreCase))));

    public override bool Equals(object? obj) => Equals(obj as Client);

    public override int GetHashCode() => Id.GetHashCode();

    public override string ToString() => Name;
}
