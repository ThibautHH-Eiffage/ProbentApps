using Microsoft.Extensions.Configuration;

namespace ProbentApps.Configuration;

public static class IConfigurationManagerExtensions
{
    public static void AddProductionConfiguration(this IConfigurationManager configuration)
    {
        bool isProxied = bool.Parse(configuration["IsProxied"] ?? bool.FalseString);

        configuration.AddJsonFile($"appsettings.{(isProxied ? "ReverseProxied" : "EdgeServer")}.json",
            optional: false, reloadOnChange: false);
    }

    public static void AddDevelopmentConfiguration(this IConfigurationManager configuration)
    {
    }
}
