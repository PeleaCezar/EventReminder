using BackgroundTasks.Services;
using BackgroundTasks.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BackgroundTasks.Tasks
{
    internal class PersonalEventNotificationsProducerBackgroundService : BackgroundService
    {
        private readonly ILogger<PersonalEventNotificationsProducerBackgroundService> _logger;
        private readonly BackgroundTaskSettings _backgroundTaskSettings;
        private readonly IServiceProvider _serviceProvider;

        public PersonalEventNotificationsProducerBackgroundService(
            ILogger<PersonalEventNotificationsProducerBackgroundService> logger,
            IOptions<BackgroundTaskSettings> backgroundTaskSettingsOptions,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _backgroundTaskSettings = backgroundTaskSettingsOptions.Value;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug($"{nameof(PersonalEventNotificationsProducerBackgroundService)} is starting.");

            stoppingToken.Register(() => _logger.LogDebug($"{nameof(PersonalEventNotificationsProducerBackgroundService)} background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug($"{nameof(PersonalEventNotificationsProducerBackgroundService)} background task is doing background work.");

                await ProducePersonalEventNotificationsAsync(stoppingToken);

                await Task.Delay(_backgroundTaskSettings.SleepTimeInMilliseconds, stoppingToken);
            }

            _logger.LogDebug($"{nameof(PersonalEventNotificationsProducerBackgroundService)} background task is stopping.");

            await Task.CompletedTask;
        }

        private async Task ProducePersonalEventNotificationsAsync(CancellationToken stoppingToken)
        {
            try
            {
                using IServiceScope scope = _serviceProvider.CreateScope();

                var personalEventNotificationsProducer = scope.ServiceProvider.GetRequiredService<IPersonalEventNotificationsProducer>();
                
                await personalEventNotificationsProducer.ProduceAsync(_backgroundTaskSettings.PersonalEventsBatchSize, stoppingToken);
            }
            catch(Exception ex)
            {
                _logger.LogCritical($"ERROR: Failed to process the batch of events: {ex.Message}", ex.Message);
            }
        }
    }
}