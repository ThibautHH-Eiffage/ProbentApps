using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProbentApps.Services.Database.Abstractions.Contexts;
using ProbentApps.Model;

namespace ProbentApps.Services.Identity.Data;

/// <summary>
/// A user store that supports lookup of users based on hashed data.
/// </summary>
/// <typeparam name="TUser">A <see cref="IdentityUser{TKey}"/>-based user class implementing <see cref="IHashedNormalizationUser"/>.</typeparam>
/// <inheritdoc/>
public class LookupHashingUserStore<TUser, TRole, TConext, TKey>(TConext context, IdentityErrorDescriber? describer = null)
    : UserStore<TUser, TRole, TConext, TKey>(context, describer)
    where TUser : IdentityUser<TKey>, IHashedNormalizationUser
    where TRole : IdentityRole<TKey>
    where TConext : DbContext
    where TKey : IEquatable<TKey>
{
    public override Task<TUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        return Users.SingleOrDefaultAsync(u => u.NormalizedUserName != null
            && u.NormalizedUserName == (u.NormalizationSalt != null
                ? Convert.ToBase64String(
                    SHA512.HashData(
                        IIdentityDbFunctions.ConcatenateBytes(
                            IIdentityDbFunctions.GetASCIIBytes(normalizedUserName),
                            u.NormalizationSalt)))
                : normalizedUserName),
            cancellationToken);
    }

    public override Task<TUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        return Users.SingleOrDefaultAsync(u => u.NormalizedEmail != null
            && u.NormalizedEmail == (u.NormalizationSalt != null
                ? Convert.ToBase64String(
                    SHA512.HashData(
                        IIdentityDbFunctions.ConcatenateBytes(
                            IIdentityDbFunctions.GetASCIIBytes(normalizedEmail),
                            u.NormalizationSalt)))
                : normalizedEmail),
            cancellationToken);
    }
}
