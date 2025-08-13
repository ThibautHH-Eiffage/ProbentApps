using Microsoft.EntityFrameworkCore;

namespace ProbentApps.Model;

[PrimaryKey($"{nameof(Structure)}{nameof(Model.Structure.Id)}", nameof(StartDate))]
public class StructureManagement
{
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Structure Structure { get; set; } = default!;

    public DateOnly StartDate { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public ApplicationUser? Manager { get; set; }
}
