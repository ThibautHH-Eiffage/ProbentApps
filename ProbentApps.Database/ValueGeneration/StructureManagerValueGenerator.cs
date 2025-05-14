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

        var managerProperty = entry.Property(static s => s.Manager);

        if (entry.State == EntityState.Added || managerProperty.IsModified)
        {
            entry.Context.Add(new StructureManagement
            {
                Manager = managerProperty.CurrentValue,
                Structure = entry.Entity,
                StartDate = DateOnly.FromDateTime(DateTime.Now)
            });
        }

        return managerProperty.CurrentValue?.Id;
    }
}
