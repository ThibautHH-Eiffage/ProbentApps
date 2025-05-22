using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ProbentApps.Services.Data.Identity;

public class LookupProtectorKeyRing(TimeProvider timeProvider,
    IOptions<KeyManagementOptions> options,
    IKeyManager keyManager) : ILookupProtectorKeyRing
{
    public string CurrentKeyId
    {
        get
        {
            var now = timeProvider.GetUtcNow();
            return (keyManager.GetAllKeys().FirstOrDefault(k => !k.IsRevoked
                        && k.ActivationDate <= now && now <= k.ExpirationDate)
                    ?? keyManager.CreateNewKey(now, now + options.Value.NewKeyLifetime))
                .KeyId.ToString();
        }
    }

    public string this[string keyId] => keyManager.GetAllKeys().Single(k => k.KeyId.ToString() == keyId).KeyId.ToString();

    public IEnumerable<string> GetAllKeyIds() => keyManager.GetAllKeys().Select(k => k.KeyId.ToString());
}
