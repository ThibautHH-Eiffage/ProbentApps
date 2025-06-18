using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.StaticFiles;
using ProbentApps.Services.Database.Abstractions.Contexts;

namespace ProbentApps.Services.Data;

public class ApplicationDataContentTypeProvider(ApplicationDbContext context) : IContentTypeProvider
{
    public bool TryGetContentType(string subpath, [MaybeNullWhen(false)] out string contentType)
    {
        contentType = string.Empty;
        return true;
    }
}
