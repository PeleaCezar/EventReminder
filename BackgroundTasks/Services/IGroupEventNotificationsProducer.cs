namespace BackgroundTasks.Services
{
    internal interface IGroupEventNotificationsProducer
    {
        Task ProduceAsync(int batchSize, CancellationToken cancellationToken = default);
    }
}
