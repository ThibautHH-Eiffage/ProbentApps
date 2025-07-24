using Microsoft.AspNetCore.Components;

namespace ProbentApps.Components;

public class AuthorizationRedirect : ComponentBase
{
    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    [Parameter]
    public bool IsAuthenticated { get; set; }

    protected override void OnInitialized() => NavigationManager.NavigateTo(IsAuthenticated
        ? "Account/AccessDenied" : $"Account/Login?returnUrl={Uri.EscapeDataString(NavigationManager.Uri)}",
        forceLoad: true);
}
