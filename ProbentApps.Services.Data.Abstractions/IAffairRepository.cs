using ProbentApps.Model;
using System.Security.Claims;

namespace ProbentApps.Services.Data.Abstractions;

public interface IAffairRepository : IRepository<Affair>
{
    IAsyncEnumerable<Affair> GetAffairsFor(ClaimsPrincipal user, bool archived);
}
