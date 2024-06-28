using BackgroundTasks.Services;
using BackgroundTasks.Settings;
using BackgroundTasks.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BackgroundTasks
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers the necessary services with the DI framework.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The same service collection.</returns>
        public static IServiceCollection AddBackgroundTasks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.Configure<BackgroundTaskSettings>(configuration.GetSection(BackgroundTaskSettings.SettingsKey));

            services.AddHostedService<GroupEventNotificationsProducerBackgroundService>();

            services.AddHostedService<PersonalEventNotificationsProducerBackgroundService>();

            services.AddHostedService<EmailNotificationConsumerBackgroundService>();
           
            services.AddHostedService<IntegrationEventConsumerBackgroundService>();

            services.AddScoped<IGroupEventNotificationsProducer, GroupEventNotificationsProducer>();

            services.AddScoped<IPersonalEventNotificationsProducer, PersonalEventNotificationsProducer>();

            services.AddScoped<IEmailNotificationsConsumer, EmailNotificationsConsumer>();

            services.AddScoped<IIntegrationEventConsumer, IntegrationEventConsumer>();

            return services;
        }
    }
}