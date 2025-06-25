using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProbentApps.Model;

namespace ProbentApps.Services.Identity.Data;

/// <summary>
/// A user manager that cryptographically hashes user lookup data, allowing data protection and indexed lookup in the backing store.
/// </summary>
/// <typeparam name="TUser">A <see cref="IdentityUser{TKey}"/>-based user class implementing <see cref="IHashedNormalizationUser"/>.</typeparam>
/// <inheritdoc/>
public class LookupHashingUserManager<TUser>(
    IUserStore<TUser> store,
    IOptions<IdentityOptions> optionsAccessor,
    IPasswordHasher<TUser> passwordHasher,
    IEnumerable<IUserValidator<TUser>> userValidators,
    IEnumerable<IPasswordValidator<TUser>> passwordValidators,
    ILookupNormalizer keyNormalizer,
    IdentityErrorDescriber errors,
    IServiceProvider services,
    ILogger<UserManager<TUser>> logger)
    : UserManager<TUser>(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    where TUser : class, IHashedNormalizationUser
{
    /// <summary>
    /// Hashes the provided data using a user-specific salt if personal data protection is enabled.
    /// </summary>
    /// <param name="user">The user whose lookup data is being protected.</param>
    /// <param name="data">The lookup data to be protected.</param>
    /// <returns>Returns the hashed salted lookup data as a base64 string or the data as plaintext if personal data protection is disabled.</returns>
    [return: NotNullIfNotNull(nameof(data))]
    private string? HashLookupData(TUser user, string? data)
    {
        if (data is null || !Options.Stores.ProtectPersonalData)
            return data;

        if (user.NormalizationSalt is null)
        {
            user.NormalizationSalt = new byte[SHA512.HashSizeInBytes];
            RandomNumberGenerator.Create().GetBytes(user.NormalizationSalt);
        }

        return Convert.ToBase64String(SHA512.HashData([..Encoding.UTF8.GetBytes(data), ..user.NormalizationSalt]));
    }

    public override Task<TUser?> FindByNameAsync(string userName)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(userName);

        return Store.FindByNameAsync(NormalizeName(userName), CancellationToken);
    }

    public override Task<TUser?> FindByEmailAsync(string email)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(email);

        return (Store as IUserEmailStore<TUser>)?.FindByEmailAsync(NormalizeEmail(email), CancellationToken) ?? Task.FromResult<TUser?>(null);
    }

    public override async Task UpdateNormalizedUserNameAsync(TUser user) =>
        await Store.SetNormalizedUserNameAsync(user,
            HashLookupData(user, NormalizeName(await GetUserNameAsync(user).ConfigureAwait(false))), CancellationToken)
        .ConfigureAwait(false);

    public override async Task UpdateNormalizedEmailAsync(TUser user) =>
        await ((Store as IUserEmailStore<TUser>)?.SetNormalizedEmailAsync(user,
            HashLookupData(user, NormalizeName(await GetEmailAsync(user).ConfigureAwait(false))), CancellationToken)
        ?? Task.CompletedTask).ConfigureAwait(false);
}
