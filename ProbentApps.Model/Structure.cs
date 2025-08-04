using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ProbentApps.Model;

[Index(nameof(Code), IsUnique = true)]
public class Structure : IEntity
{
    public Guid Id { get; set; }

    [MaxLength(64)]
    public required string Name { get; set; }

    [MaxLength(128)]
    [Unicode(false)]
    public required string Code { get; set; }

    public required IList<StructureManagement> Managements { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public ApplicationUser? Manager { get; set; }

    public bool IsActive => Manager is not null;
}
