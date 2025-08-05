using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using UUIDNext;

namespace ProbentApps.Services.Database;

internal class UUIDv7GuidValueGenerator : GuidValueGenerator
{
    public override Guid Next(EntityEntry entry) => Uuid.NewDatabaseFriendly(UUIDNext.Database.SqlServer);
}
