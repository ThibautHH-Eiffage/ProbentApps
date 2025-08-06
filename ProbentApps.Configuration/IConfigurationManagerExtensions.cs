using Microsoft.Extensions.Configuration;

namespace ProbentApps.Configuration;

public static class IConfigurationManagerExtensions
{
    public static bool IsReverseProxyingEnabled(this IConfiguration configuration) =>
        bool.Parse(configuration["IsProxied"] ?? bool.FalseString);

    public static void AddProductionConfiguration(this IConfigurationManager configuration)
    {
        configuration.AddJsonFile($"appsettings.{(configuration.IsReverseProxyingEnabled() ? "ReverseProxied" : "EdgeServer")}.json",
            optional: false, reloadOnChange: false);
    }

    public static void AddDevelopmentConfiguration(this IConfigurationManager configuration)
    {
    }
}
