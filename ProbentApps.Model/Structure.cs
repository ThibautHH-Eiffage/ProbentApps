using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ProbentApps.Model;

[Index(nameof(Code), IsUnique = true)]
[Index(nameof(ShortCode))]
public class Structure : IEntity
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

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public StructureType StructureType { get; set; } = default!;
}
