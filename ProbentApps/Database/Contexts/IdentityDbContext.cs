using System.Linq.Expressions;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using ProbentApps.Data;

namespace ProbentApps.Database.Contexts;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("identity");

        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

        builder.HasDbFunction(() => Convert.ToBase64String(Array.Empty<byte>()))
            .HasName("BASE64_ENCODE")
            .HasSchema("identity");
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
