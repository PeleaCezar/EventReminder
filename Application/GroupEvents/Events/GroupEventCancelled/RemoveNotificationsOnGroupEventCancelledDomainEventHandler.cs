using Application.Core.Abstractions.Common;
using Domain.Core.Events;
using Domain.Events;
using Domain.Repositories;

namespace Application.GroupEvents.Events.GroupEventCancelled;

/// <summary>
/// Represents the <see cref="GroupEventCancelledDomainEvent"/> class.
/// </summary>
internal sealed class RemoveNotificationsOnGroupEventCancelledDomainEventHandler : IDomainEventHandler<GroupEventCancelledDomainEvent>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IDateTime _dateTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveNotificationsOnGroupEventCancelledDomainEventHandler"/> class.
    /// </summary>
    /// <param name="notificationRepository">The notification repository.</param>
    /// <param name="dateTime">The date and time.</param>
    public RemoveNotificationsOnGroupEventCancelledDomainEventHandler(INotificationRepository notificationRepository, IDateTime dateTime)
    {
        _notificationRepository = notificationRepository;
        _dateTime = dateTime;
    }

    public async Task Handle(GroupEventCancelledDomainEvent notification, CancellationToken cancellationToken) =>
        await _notificationRepository.RemoveNotificationsForEventAsync(notification.GroupEvent, _dateTime.UtcNow);
}
