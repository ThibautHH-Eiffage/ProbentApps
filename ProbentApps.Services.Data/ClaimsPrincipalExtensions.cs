using System.Security.Claims;

namespace ProbentApps.Services.Data;

internal static class ClaimsPrincipalExtensions
{
    public static Guid[] GetExtraManagedAffairs(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);

        const string ExtraManagedAffairsClaimType = "ExtraManagedAffairs";

        return principal.FindAll(ExtraManagedAffairsClaimType)
            .SelectMany(static c => c.Value
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(Guid.Parse))
            .ToArray();
    }
}
