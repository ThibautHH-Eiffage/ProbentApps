using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProbentApps.Model;

public class Structure
{
    public Guid Id { get; set; }

    [MaxLength(64)]
    public required string Name { get; set; }

    [MaxLength(32)]
    [Column(TypeName = "varchar(32)")]
    public required string Code { get; set; }

    public required Structure? Parent { get; set; }

    public required IList<StructureManagement> Managements { get; set; }

    public ApplicationUser? Manager { get; set; }

    public bool IsActive => Manager is not null;
}
