using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HostFiltering;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ProbentApps.Services.Options;

internal class ForwardedHeadersOptionsConfiguration(IOptions<HostFilteringOptions> hostFilteringOptions, IConfiguration configuration)
    : NamedConfigureFromConfigurationOptions<ForwardedHeadersOptions>(null, configuration.GetSection(nameof(ForwardedHeaders)))
{
    public override void Configure(string? name, ForwardedHeadersOptions options)
    {
        base.Configure(name, options);

        if (options.AllowedHosts is not null && options.AllowedHosts.Any())
            return;

        options.AllowedHosts = hostFilteringOptions.Value.AllowedHosts;
    }
}
