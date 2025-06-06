using Microsoft.AspNetCore.StaticFiles;
using ProbentApps.Database.Contexts;
using System.Diagnostics.CodeAnalysis;

namespace ProbentApps.Services.Data;

public class ApplicationDataContentTypeProvider(ApplicationDbContext context) : IContentTypeProvider
{
    public bool TryGetContentType(string subpath, [MaybeNullWhen(false)] out string contentType)
    {
        contentType = string.Empty;
        return true;
    }
}
