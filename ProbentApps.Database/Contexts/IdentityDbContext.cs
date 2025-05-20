using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using ProbentApps.Model;

namespace ProbentApps.Database.Contexts;

public abstract class IdentityDbContext(DbContextOptions options) : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options), IDbContext
{
    public static string Schema => "identity";

    internal static string Base64EncodeFunctionName => "BASE64_ENCODE";

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(Schema);

        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>()
            .Ignore(static u => u.ManagedStructures)
            .ToTable("Users");
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
            .HasTranslation(HashBytesWithSHA(SHA512.HashSizeInBits));
        builder.HasDbFunction(() => IIdentityDbFunctions.ConcatenateBytes(Array.Empty<byte>(), Array.Empty<byte>()))
            .HasTranslation(ConcatenateByteArrays(256 + SHA512.HashSizeInBytes));
        builder.HasDbFunction(() => IIdentityDbFunctions.GetASCIIBytes(string.Empty))
            .HasTranslation(EncodeToASCII);
    }

    public abstract Func<IReadOnlyList<SqlExpression>, SqlExpression> EncodeToASCII { get; }

    public abstract Func<IReadOnlyList<SqlExpression>, SqlExpression> HashBytesWithSHA(ushort size);

    public abstract Func<IReadOnlyList<SqlExpression>, SqlExpression> ConcatenateByteArrays(ushort resultMaxLength);
}

public class IdentityDbContext<TFunctions>(DbContextOptions<IdentityDbContext<TFunctions>> options) : IdentityDbContext(options)
    where TFunctions : IIdentityDbFunctions
{
    public override Func<IReadOnlyList<SqlExpression>, SqlExpression> EncodeToASCII => TFunctions.EncodeToASCII;

    public override Func<IReadOnlyList<SqlExpression>, SqlExpression> ConcatenateByteArrays(ushort resultMaxLength) => TFunctions.ConcatenateByteArrays(resultMaxLength);

    public override Func<IReadOnlyList<SqlExpression>, SqlExpression> HashBytesWithSHA(ushort size) => TFunctions.HashBytesWithSHA(size);
}
