using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace ProbentApps.Services.Options;

internal class ApplicationStaticFilesOptionsConfiguration(
    [FromKeyedServices(ApplicationStaticFilesOptionsConfiguration.ApplicationDataFilesKey)] IFileProvider applicationDataFileProvider,
    [FromKeyedServices(ApplicationStaticFilesOptionsConfiguration.ApplicationDataFilesKey)] IContentTypeProvider applicationDataContentTypeProvider)
    : ConfigureNamedOptions<StaticFileOptions, IFileProvider, IContentTypeProvider>(ApplicationDataFilesKey,
        applicationDataFileProvider,
        applicationDataContentTypeProvider,
        ConfigureFiles)
{
    private const string ApplicationDataFilesKey = "ApplicationDataFiles";

    private static void ConfigureFiles(StaticFileOptions options, IFileProvider fileProvider, IContentTypeProvider contentTypeProvider)
    {
        options.ContentTypeProvider = contentTypeProvider;
        options.FileProvider = fileProvider;
    }
}
