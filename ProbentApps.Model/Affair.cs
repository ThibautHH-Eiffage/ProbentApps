using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProbentApps.Model;

[Table("Affairs")]
public class Affair : Structure
{
    public bool IsArchived { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public required Client Client { get; set; }

    public required IList<Order> Orders { get; set; }
}
