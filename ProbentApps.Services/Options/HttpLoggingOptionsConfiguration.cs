using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;

namespace ProbentApps.Services.Options;

internal class HttpLoggingOptionsConfiguration() : IConfigureOptions<HttpLoggingOptions>
{
    void IConfigureOptions<HttpLoggingOptions>.Configure(HttpLoggingOptions options) =>
        options.LoggingFields |= HttpLoggingFields.Duration;
}
