using System.ComponentModel.DataAnnotations;

namespace ProbentApps.Services.Email;

public record EmailOptions
{
    public EmailOptions() => SmtpPort = SmtpPort == default ? (UseTls ? 465 : 25) : SmtpPort;

    [Required]
    public required string SmtpHost { get; set; }

    [Range(1, 65535)]
    public int SmtpPort { get; set; }

    public string? SmtpUsername { get; set; }

    public string? SmtpPassword { get; set; }

    public bool UseTls { get; set; } = true;

    [Required]
    [EmailAddress]
    public required string SenderAddress { get; set; }

    public string? SenderName { get; set; }
}
