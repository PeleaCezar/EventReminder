using BackgroundTasks.Services;
using BackgroundTasks.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BackgroundTasks.Tasks;

/// <summary>
/// Represents the email notification consumer background service.
/// </summary>
internal sealed class EmailNotificationConsumerBackgroundService : BackgroundService
{
    private readonly ILogger<EmailNotificationConsumerBackgroundService> _logger;
    private readonly BackgroundTaskSettings _backgroundTaskSettings;
    private readonly IServiceProvider _serviceProvider;

    public EmailNotificationConsumerBackgroundService(
    ILogger<EmailNotificationConsumerBackgroundService> logger,
    IOptions<BackgroundTaskSettings> backgroundTaskSettingsOptions,
    IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _backgroundTaskSettings = backgroundTaskSettingsOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug($"{nameof(EmailNotificationConsumerBackgroundService)} is starting");

        stoppingToken.Register(() => _logger.LogDebug($"{nameof(EmailNotificationConsumerBackgroundService)} background task is stopping."));
  
       while(!stoppingToken.IsCancellationRequested)
       {
            _logger.LogDebug($"{nameof(EmailNotificationConsumerBackgroundService)} background task is doing background work.");

            await ConsumeEventNotificationsAsync(stoppingToken);

            await Task.Delay(_backgroundTaskSettings.SleepTimeInMilliseconds, stoppingToken);
       }

        _logger.LogDebug($"{nameof(EmailNotificationConsumerBackgroundService)} background task is stopping.");

        await Task.CompletedTask;
    }

    /// <summary>
    /// Consumes the next batch of event notifications.
    /// </summary>
    /// <param name="stoppingToken">The stopping token.</param>
    /// <returns>The completed task.</returns>
    private async Task ConsumeEventNotificationsAsync(CancellationToken stoppingToken)
    {
        //If IEmailNotificationsConsumer is defined as a scoped service,
        //directly injecting it into a singleton service would violate its intended lifecycle,
        //leading to potential issues such as memory leaks, stale data,
        //or incorrect behavior due to improper reuse of the instance.

        try
        {
            using IServiceScope scope = _serviceProvider.CreateScope();

            var emailNotificationsConsumer = scope.ServiceProvider.GetRequiredService<IEmailNotificationsConsumer>();

            await emailNotificationsConsumer.ConsumeAsync(
                _backgroundTaskSettings.NotificationsBatchSize,
                _backgroundTaskSettings.AllowedNotificationTimeDiscrepancyInMinutes,
                stoppingToken);
        }
        catch (Exception e)
        {
            _logger.LogCritical($"ERROR: Failed to process the batch of notifications: {e.Message}", e.Message);
        }
    }
}
