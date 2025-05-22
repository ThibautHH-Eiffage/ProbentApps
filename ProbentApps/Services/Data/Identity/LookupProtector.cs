using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;

namespace ProbentApps.Services.Data.Identity;

public class LookupProtector(IDataProtectionProvider dataProtectionProvider) : ILookupProtector
{
    [return: NotNullIfNotNull(nameof(data))]
    public string? Protect(string keyId, string? data)
    {
        if (data is null)
            return null;
        return dataProtectionProvider.CreateProtector(nameof(LookupProtector)).Protect(data);
    }

    [return: NotNullIfNotNull(nameof(data))]
    public string? Unprotect(string keyId, string? data)
    {
        if (data is null)
            return null;
        return dataProtectionProvider.CreateProtector(nameof(LookupProtector)).Unprotect(data);
    }
}
