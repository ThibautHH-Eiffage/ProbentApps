using System.ComponentModel.DataAnnotations;

namespace ProbentApps.Model;

public class Client
{
    public Guid Id { get; set; }

    [MaxLength(128)]
    public required string Name { get; set; }

    [MaxLength(64)]
    public string? MainProviderId { get; set; }

    public required string[] ExtraProviderIds { get; set; }
}
