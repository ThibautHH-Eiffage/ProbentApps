namespace ProbentApps.Services.Database.Abstractions.Contexts;

public interface IDbContext
{
    public abstract static string Schema { get; }
}
