using System.Security.Claims;

namespace ProbentApps.Services.Data;

internal static class ClaimsPrincipalExtensions
{
    public static Guid[] GetExtraManagedStructures(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);

        const string ExtraManagedStructuresClaimType = "ExtraManagedStructures";

        return principal.FindAll(ExtraManagedStructuresClaimType)
            .SelectMany(static c => c.Value
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(Guid.Parse))
            .ToArray();
    }
}
