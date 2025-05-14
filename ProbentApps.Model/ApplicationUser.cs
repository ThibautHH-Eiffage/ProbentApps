using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ProbentApps.Model;

[Table("Users", Schema = "identity")]
public class ApplicationUser : IdentityUser<Guid>, IHashedNormalizationUser
{
    public static readonly Guid RootId = new(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);

    [Column(TypeName = "binary(64)")]
    public byte[]? NormalizationSalt { get; set; }

    public required IList<Structure> ManagedStructures { get; set; }
}
