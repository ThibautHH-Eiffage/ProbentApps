using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HostFiltering;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ProbentApps.Configuration;

namespace ProbentApps.Services.Options;

internal class ForwardedHeadersOptionsConfiguration(IOptions<HostFilteringOptions> hostFilteringOptions, IConfiguration configuration)
    : IConfigureOptions<ForwardedHeadersOptions>
{
    public void Configure(ForwardedHeadersOptions options)
    {
        var forwardedHeaders = configuration.GetRequiredSection(nameof(ForwardedHeaders));

        foreach (string host in forwardedHeaders[nameof(options.AllowedHosts)]?
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            ?? [])
            options.AllowedHosts.Add(host);

        if (!options.AllowedHosts.Any())
            options.AllowedHosts = hostFilteringOptions.Value.AllowedHosts;

        bool cleared = false;
        foreach (string ip in forwardedHeaders[nameof(options.KnownProxies)]?
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            ?? [])
        {
            if (!cleared)
            {
                cleared = true;
                options.KnownProxies.Clear();
            }
            options.KnownProxies.Add(IPAddress.Parse(ip));
        }

        if (configuration.IsReverseProxyingEnabled())
            options.ForwardedHeaders = ForwardedHeaders.All & ~ForwardedHeaders.XForwardedPrefix;
        options.KnownNetworks.Clear();
        options.RequireHeaderSymmetry = true;
    }
}
