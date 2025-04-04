using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ProbentApps.Data;

public class ApplicationUser : IdentityUser<Guid>, IHashedNormalizationUser
{
    [Column(TypeName = "binary(64)")]
    public byte[]? NormalizationSalt { get; set; }
}
