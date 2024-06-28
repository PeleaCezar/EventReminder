using Application.Core.Abstractions.Common;
using Application.Core.Abstractions.Data;
using Domain.Repositories;

namespace BackgroundTasks.Services;

/// <summary>
/// Represents the personal event notifications producer.
/// </summary>
internal sealed class PersonalEventNotificationsProducer : IPersonalEventNotificationsProducer
{
    private readonly IPersonalEventRepository _personalEventRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IDateTime _dateTime;
    private readonly IUnitOfWork _unitOfWork;

    public Task ProduceAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
