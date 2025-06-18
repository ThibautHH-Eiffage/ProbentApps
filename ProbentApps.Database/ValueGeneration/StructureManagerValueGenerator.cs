using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using ProbentApps.Model;

namespace ProbentApps.Database.ValueGeneration;

internal class StructureManagerValueGenerator : ValueGenerator<Guid?>
{
    public override bool GeneratesTemporaryValues => false;

    public override Guid? Next(EntityEntry e)
    {
#pragma warning disable EF1001
        EntityEntry<Structure> entry = new(e.GetInfrastructure());
#pragma warning restore EF1001

        var managerProperty = entry.Navigation(nameof(Structure.Manager));
        var manager = (ApplicationUser?)managerProperty.CurrentValue;

        if (manager is null && !managerProperty.IsLoaded
            && entry.State != EntityState.Added)
            return entry.Property<Guid?>($"{nameof(Structure.Manager)}{nameof(ApplicationUser.Id)}").OriginalValue;

        if (entry.State == EntityState.Added || managerProperty.IsModified)
        {
            entry.Context.Add(new StructureManagement
            {
                Manager = manager,
                Structure = entry.Entity,
                StartDate = DateOnly.FromDateTime(DateTime.Now)
            });
        }

        return manager?.Id;
    }
}
