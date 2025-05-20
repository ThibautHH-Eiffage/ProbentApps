using System.ComponentModel.DataAnnotations;

namespace ProbentApps.Model;

public class Affair
{
    public Guid Id { get; set; }

    [MaxLength(128)]
    public required string Name { get; set; }

    [MaxLength(64)]
    public required string ProviderId { get; set; }

    public required Structure Structure { get; set; }

    public required Client Client { get; set; }

    public required IList<Order> Orders { get; set; }
}
