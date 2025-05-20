using System.ComponentModel.DataAnnotations.Schema;

namespace ProbentApps.Model;

public class StructureManagement
{
    public required Structure Structure { get; set; }

    public ApplicationUser? Manager { get; set; }

    public DateOnly StartDate { get; set; }
}
