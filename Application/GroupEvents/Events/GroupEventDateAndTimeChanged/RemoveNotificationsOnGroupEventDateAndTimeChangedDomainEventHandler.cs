using Application.Core.Abstractions.Common;
using Domain.Core.Events;
using Domain.Events;
using Domain.Repositories;

namespace Application.GroupEvents.Events.GroupEventDateAndTimeChanged;

/// <summary>
/// Represents the <see cref="GroupEventDateAndTimeChangedDomainEvent"/> class.
/// </summary>
internal sealed class RemoveNotificationsOnGroupEventDateAndTimeChangedDomainEventHandler
    : IDomainEventHandler<GroupEventDateAndTimeChangedDomainEvent>
{
    private readonly INotificationRepository _dbContext;
    private readonly IDateTime _dateTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveNotificationsOnGroupEventDateAndTimeChangedDomainEventHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The notification repository.</param>
    /// <param name="dateTime">The date and time.</param>
    public RemoveNotificationsOnGroupEventDateAndTimeChangedDomainEventHandler(INotificationRepository dbContext, IDateTime dateTime)
    {
        _dbContext = dbContext;
        _dateTime = dateTime;
    }

    /// <inheritdoc />
    public async Task Handle(GroupEventDateAndTimeChangedDomainEvent notification, CancellationToken cancellationToken) =>
        await _dbContext.RemoveNotificationsForEventAsync(notification.GroupEvent, _dateTime.UtcNow);
}