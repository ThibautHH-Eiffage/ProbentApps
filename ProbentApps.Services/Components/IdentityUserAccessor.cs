using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using ProbentApps.Model;

namespace ProbentApps.Services.Components;

public sealed class IdentityUserAccessor(SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    IdentityRedirectManager redirectManager)
{
    public async Task<ApplicationUser> GetRequiredUserAsync(HttpContext context)
    {
        if (await userManager.GetUserAsync(context.User) is ApplicationUser user)
        {
            return user;
        }

        await signInManager.SignOutAsync();
        redirectManager.RedirectToWithStatus("Account/Login",
            $"Error: Invalid credentials, please log in again. (ID '{userManager.GetUserId(context.User)}')",
            context);
        throw new InvalidOperationException($"{nameof(IdentityUserAccessor)} can only be used during static rendering.");
    }
}
