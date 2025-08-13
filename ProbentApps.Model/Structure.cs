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
    public required string Name { get; set; }

    [MaxLength(128)]
    [Unicode(false)]
    public required string Code { get; set; }

    public string ParentCode => Code[..(Code.Length - ShortCode.Length - 1)];

    [Unicode(false)]
    public required string ShortCode { get; set; }

    public required IList<StructureManagement> Managements { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public ApplicationUser? Manager { get; set; }

    public bool IsActive => Manager is not null;
}
