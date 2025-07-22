using Microsoft.Extensions.Localization;
using ProbentApps.Localization;

namespace ProbentApps.Services.Components;

public sealed class IdentityLocalizer(IStringLocalizer<SharedResources> sharedLocalizer,
    IStringLocalizer<AccountResources> accountLocalizer,
    IStringLocalizer<IdentityErrorResources> errorLocalizer) : IStringLocalizer<AccountResources>
{
    public LocalizedString this[string name] => accountLocalizer[name];

    public LocalizedString this[string name, params object[] arguments] => accountLocalizer[name, arguments];

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => accountLocalizer.GetAllStrings(includeParentCultures);

    public string GetError(string message, params object[] args) => $"{sharedLocalizer["Error"]}{sharedLocalizer[": "]}{errorLocalizer[message, args]}";
}
