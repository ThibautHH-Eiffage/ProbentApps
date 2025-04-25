using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ProbentApps.Model;

[Table("Users", Schema = "identity")]
public class ApplicationUser : IdentityUser<Guid>, IHashedNormalizationUser
{
    [Column(TypeName = "binary(64)")]
    public byte[]? NormalizationSalt { get; set; }
}
