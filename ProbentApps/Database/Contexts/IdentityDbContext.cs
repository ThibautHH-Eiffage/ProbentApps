using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using ProbentApps.Data;

namespace ProbentApps.Database.Contexts;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options), IDbContext
{
    public static string Schema => "identity";

    internal static string Base64EncodeFunctionName => "BASE64_ENCODE";

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(Schema);

        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

        builder.HasDbFunction(() => Convert.ToBase64String(Array.Empty<byte>()))
            .HasName(Base64EncodeFunctionName)
            .HasSchema(Schema);
        builder.HasDbFunction(() => SHA512.HashData(Array.Empty<byte>()))
            .HasTranslation(Functions.HashBytesWithSHA(SHA512.HashSizeInBits));
        builder.AddByteArrayConcatenation(new SqlServerByteArrayTypeMapping(null, 256 + SHA512.HashSizeInBytes));
        builder.HasDbFunction(() => Functions.ASCIIEncode(string.Empty))
            .HasTranslation(args =>
                Functions.ConvertTo(new SqlServerByteArrayTypeMapping(null, 256))([
                    Functions.ConvertTo(new SqlServerStringTypeMapping(null, false, 256))(args)
            ]));
    }
}
