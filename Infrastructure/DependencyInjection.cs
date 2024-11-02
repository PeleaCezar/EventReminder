using Application.Core.Abstractions.Authentication;
using Application.Core.Abstractions.Common;
using Application.Core.Abstractions.Cryptography;
using Application.Core.Abstractions.Emails;
using Application.Core.Abstractions.Messaging;
using Application.Core.Abstractions.Notifications;
using Domain.Services;
using Infrastructure.Authentication;
using Infrastructure.Authentication.Settings;
using Infrastructure.Common;
using Infrastructure.Cryptography;
using Infrastructure.Emails;
using Infrastructure.Emails.Settings;
using Infrastructure.Messaging;
using Infrastructure.Messaging.Settings;
using Infrastructure.Notifications;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the necessary services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:SecurityKey"]))
            });

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SettingsKey));

        services.Configure<MailSettings>(configuration.GetSection(MailSettings.SettingsKey));

        services.Configure<MessageBrokerSettings>(configuration.GetSection(MessageBrokerSettings.SettingsKey));

        services.AddScoped<IUserIdentifierProvider, UserIdentifierProvider>();

        services.AddScoped<IJwtProvider, JwtProvider>();

        services.AddTransient<IDateTime, MachineDateTime>();

        services.AddTransient<IPasswordHasher, PasswordHasher>();

        services.AddTransient<IPasswordHashChecker, PasswordHasher>();

        services.AddTransient<IEmailService, EmailService>();

        services.AddTransient<IEmailNotificationService, EmailNotificationService>();

        services.AddSingleton<IIntegrationEventPublisher, IntegrationEventPublisher>();

        return services;
    }

}
