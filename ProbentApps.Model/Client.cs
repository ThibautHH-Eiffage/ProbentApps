using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ProbentApps.Model;

public class Client
{
    public Guid Id { get; set; }

    [MaxLength(128)]
    public required string Name { get; set; }

    [MaxLength(64)]
    [Unicode(false)]
    public string? Code { get; set; }

    public string[] ExtraCodes { get; set; } = [];
}
