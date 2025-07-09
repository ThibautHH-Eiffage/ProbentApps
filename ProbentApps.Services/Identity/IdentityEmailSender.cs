using Microsoft.AspNetCore.Identity;
using ProbentApps.Model;
using ProbentApps.Services.Email;

namespace ProbentApps.Services.Identity;

internal class IdentityEmailSender(IEmailSender emailSender) : IEmailSender<ApplicationUser>
{
    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
        SendAsync(user, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
        SendAsync(user, "Reset your password", $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
        SendAsync(user, "Reset your password", resetCode, false);

    private Task SendAsync(ApplicationUser user, string subject, string body, bool isHtml = true) =>
        emailSender.SendAsync((user.UserName, user.Email!), subject, body, isHtml);
}
