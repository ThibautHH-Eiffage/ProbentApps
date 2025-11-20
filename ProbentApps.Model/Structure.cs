using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ProbentApps.Model;

[Index(nameof(Code), IsUnique = true)]
[Index(nameof(ShortCode))]
public class Structure : IEntity, IEquatable<Structure>
{
    public const char CodeSeparator = '|';

    public Guid Id { get; set; }

    [MaxLength(64)]
    public string Name { get; set; } = default!;

    [MaxLength(128)]
    [Unicode(false)]
    public string Code { get; set; } = default!;

    public string ParentCode => Code[..(Code.Length - ShortCode.Length - 1)];

    [Unicode(false)]
    public string ShortCode { get; set; } = default!;

    public IList<StructureManagement> Managements { get; set; } = [];

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public ApplicationUser? Manager { get; set; }

    public bool IsActive => Manager is not null;

    public bool Equals(Structure? other) => other is not null
        && (Id.Equals(other.Id)
            || ((Id.Equals(default) || other.Id.Equals(default))
                && Code.Equals(other.Code, StringComparison.Ordinal)));

    public override bool Equals(object? obj) => Equals(obj as Structure);

    public override int GetHashCode() => Id.GetHashCode();

    public override string ToString() => Name;
}
