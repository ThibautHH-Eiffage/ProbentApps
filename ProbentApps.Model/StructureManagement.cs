using Microsoft.EntityFrameworkCore;

namespace ProbentApps.Model;

public class StructureManagement
{
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public required Structure Structure { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public ApplicationUser? Manager { get; set; }

    public DateOnly StartDate { get; set; }
}
