using System.Security.Claims;

namespace ProbentApps.Services.Data;

internal static class ClaimsPrincipalExtensions
{
    internal const string ExtraManagedStructuresClaimType = "ExtraManagedStructures";

    public static Guid[] GetExtraManagedStructures(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);

        return principal.FindAll(ExtraManagedStructuresClaimType)
            .SelectMany(static c => c.Value
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(Guid.Parse))
            .ToArray();
    }
}
