using Microsoft.Extensions.DependencyInjection;

namespace ProbentApps.Services.Email;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddEmailServices(this IServiceCollection services)
    {
        services.AddScoped<IEmailSender, EmailSender>()
            .AddOptions<EmailOptions>()
                .BindConfiguration("Email")
                .ValidateDataAnnotations()
                .ValidateOnStart();
        return services;
    }
}
