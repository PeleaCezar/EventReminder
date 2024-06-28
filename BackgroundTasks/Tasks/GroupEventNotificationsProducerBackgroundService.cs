using BackgroundTasks.Services;
using BackgroundTasks.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BackgroundTasks.Tasks;

/// <summary>
/// Represents the background service for producing group event notifications.
/// </summary>
internal sealed class GroupEventNotificationsProducerBackgroundService : BackgroundService
{
    private readonly ILogger<GroupEventNotificationsProducerBackgroundService> _logger;
    private readonly BackgroundTaskSettings _backgroundTaskSettings;
    private readonly IServiceProvider _serviceProvider;

    public GroupEventNotificationsProducerBackgroundService(
    ILogger<GroupEventNotificationsProducerBackgroundService> logger,
    IOptions<BackgroundTaskSettings> backgroundTaskSettingsOptions,
    IServiceProvider serviceProvider)
    {
        _logger = logger;
        _backgroundTaskSettings = backgroundTaskSettingsOptions.Value;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug($"{nameof(GroupEventNotificationsProducerBackgroundService)} is starting.");

        stoppingToken.Register(() => _logger.LogDebug($"{nameof(GroupEventNotificationsProducerBackgroundService)} background task is stopping."));

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogDebug($"{nameof(GroupEventNotificationsProducerBackgroundService)} background task is doing background work.");

            await ProduceGroupEventNotificationsAsync(stoppingToken);

            await Task.Delay(_backgroundTaskSettings.SleepTimeInMilliseconds, stoppingToken);
        }

        _logger.LogDebug($"{nameof(GroupEventNotificationsProducerBackgroundService)} background task is stopping.");

        await Task.CompletedTask;
    }

    private async Task ProduceGroupEventNotificationsAsync(CancellationToken stoppingToken)
    {
        try
        {
            using IServiceScope scope = _serviceProvider.CreateScope();

            var groupEventNotificationsProducer = scope.ServiceProvider.GetRequiredService<IGroupEventNotificationsProducer>();

            await groupEventNotificationsProducer.ProduceAsync(_backgroundTaskSettings.AttendeesBatchSize, stoppingToken);
        }
        catch (Exception e)
        {
            _logger.LogCritical($"ERROR: Failed to process the batch of events: {e.Message}", e.Message);
        }
    }
}
