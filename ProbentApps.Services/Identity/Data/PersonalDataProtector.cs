using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;

namespace ProbentApps.Services.Identity.Data;

public class PersonalDataProtector(IDataProtectionProvider dataProtectionProvider) : IPersonalDataProtector
{
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("ProbentApps.Services.Data.Identity.PersonalDataProtector");

    [return: NotNullIfNotNull(nameof(data))]
    public string? Protect(string? data) => data is string plaintext ? _dataProtector.Protect(plaintext) : null;

    [return: NotNullIfNotNull(nameof(data))]
    public string? Unprotect(string? data) => data is string protectedData ? _dataProtector.Unprotect(protectedData) : null;
}
