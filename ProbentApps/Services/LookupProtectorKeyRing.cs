using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ProbentApps.Services;

public class LookupProtectorKeyRing(TimeProvider timeProvider,
    IOptions<KeyManagementOptions> options,
    IKeyManager keyManager) : ILookupProtectorKeyRing
{
    public string CurrentKeyId
    {
        get
        {
            var now = timeProvider.GetUtcNow();
            return keyManager.GetAllKeys()
                .FirstOrDefault(k => !k.IsRevoked
                    && k.ActivationDate <= now && now <= k.ExpirationDate)
                ?.KeyId.ToString()
                ?? keyManager.CreateNewKey(now, now + options.Value.NewKeyLifetime).KeyId.ToString();
        }
    }

    public string this[string keyId] => keyId;

    public IEnumerable<string> GetAllKeyIds() => keyManager.GetAllKeys().Select(k => k.KeyId.ToString());
}
