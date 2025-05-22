using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;

namespace ProbentApps.Model;

[Table("Users", Schema = "identity")]
public class ApplicationUser : IdentityUser<Guid>, IHashedNormalizationUser
{
    public static readonly Guid RootId = new(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);

    [MaxLength(SHA512.HashSizeInBytes)]
    public byte[]? NormalizationSalt { get; set; }

    public required IList<Structure> ManagedStructures { get; set; }
}
