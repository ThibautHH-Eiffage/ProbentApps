using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ProbentApps.Model;

public class Affair
{
    public Guid Id { get; set; }

    [MaxLength(128)]
    public required string Name { get; set; }

    [MaxLength(64)]
    [Unicode(false)]
    public required string Code { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public required Structure Structure { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public required Client Client { get; set; }

    public required IList<Order> Orders { get; set; }
}
