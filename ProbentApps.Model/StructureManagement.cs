using Microsoft.EntityFrameworkCore;

namespace ProbentApps.Model;

[PrimaryKey($"{nameof(Structure)}{nameof(Model.Structure.Id)}", nameof(StartDate))]
public class StructureManagement
{
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public required Structure Structure { get; set; }

    public DateOnly StartDate { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public ApplicationUser? Manager { get; set; }
}
