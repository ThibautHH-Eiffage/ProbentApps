namespace ProbentApps.Services.Email;

public interface IEmailSender
{
    Task SendAsync((string? name, string address) sender,
        string subject, string body,
        bool isHtml = false,
        CancellationToken cancellationToken = default);
}
