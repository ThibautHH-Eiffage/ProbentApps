using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ProbentApps.Model;

[Index(nameof(Code), IsUnique = true)]
public class Structure : IEntity
{
    public const char CodeSeparator = '|';

    public Guid Id { get; set; }

    [MaxLength(64)]
    public required string Name { get; set; }

    [MaxLength(128)]
    [Unicode(false)]
    public required string Code { get; set; }

    public string ParentCode => Code[..Code.LastIndexOf(CodeSeparator)];

    public string ShortCode => Code[(Code.LastIndexOf(CodeSeparator) + 1)..];

    public required IList<StructureManagement> Managements { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public ApplicationUser? Manager { get; set; }

    public bool IsActive => Manager is not null;
}
