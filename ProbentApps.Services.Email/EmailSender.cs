using System.Security.Authentication;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace ProbentApps.Services.Email;

internal class EmailSender : IEmailSender, IDisposable
{
    private readonly ILogger<EmailSender> _logger;
    private readonly EmailOptions _options;
    private readonly SmtpClient _smtpClient;

    public EmailSender(ILogger<EmailSender> logger, IOptions<EmailOptions> options)
    {
        _logger = logger;
        _options = options.Value;
        _smtpClient = new()
        {
            CheckCertificateRevocation = true,
            RequireTLS = _options.UseTls,
            SslProtocols = _options.UseTls ? SslProtocols.Tls12 | SslProtocols.Tls13 : SslProtocols.None,
        };
    }

    public void Dispose() => _smtpClient.Dispose();

    public async Task SendAsync((string? name, string address) sender,
        string subject, string body,
        bool isHtml = false,
        CancellationToken cancellationToken = default)
    {
        await EnsureReadyAsync(cancellationToken);
        var text = new TextPart(isHtml ? TextFormat.Html : TextFormat.Plain);
        text.SetText(Encoding.UTF8, body);
        _logger.LogDebug("Sending email to {EmailAddress} with subject '{Subject}'", sender.address, subject);
        await _smtpClient.SendAsync(
            new([new MailboxAddress(_options.SenderName, _options.SenderAddress)],
                [new MailboxAddress(sender.name ?? string.Empty, sender.address)],
                subject, text),
            cancellationToken);
    }

    private async Task EnsureReadyAsync(CancellationToken cancellationToken = default)
    {
        if (_smtpClient.IsConnected && _smtpClient.IsAuthenticated)
            return;
        if (!_smtpClient.IsConnected)
            await _smtpClient.ConnectAsync(_options.SmtpHost, _options.SmtpPort,
                _options.UseTls ? SecureSocketOptions.StartTls : SecureSocketOptions.None,
                cancellationToken);
        if (!_smtpClient.IsAuthenticated
            && _smtpClient.Capabilities.HasFlag(SmtpCapabilities.Authentication)
            && !string.IsNullOrEmpty(_options.SmtpUsername))
            await _smtpClient.AuthenticateAsync(_options.SmtpUsername, _options.SmtpPassword ?? string.Empty, cancellationToken);
    }
}
