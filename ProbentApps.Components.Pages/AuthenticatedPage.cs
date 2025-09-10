using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace ProbentApps.Components.Pages;

[Authorize]
public abstract class AuthenticatedPage : ComponentBase
{
    protected ClaimsPrincipal User { get; private set; } = default!;

    [CascadingParameter]
    private Task<AuthenticationState?> AuthenticationState { get; set; } = default!;

    protected override async Task OnInitializedAsync() => User = (await AuthenticationState)!.User;
}
