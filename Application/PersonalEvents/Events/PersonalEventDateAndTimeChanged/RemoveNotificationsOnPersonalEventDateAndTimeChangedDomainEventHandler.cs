using Application.Core.Abstractions.Common;
using Domain.Core.Events;
using Domain.Events;
using Domain.Repositories;

namespace Application.PersonalEvents.Events.PersonalEventDateAndTimeChanged
{
    /// <summary>
    /// Represents the <see cref="PersonalEventDateAndTimeChangedDomainEvent"/> class.
    /// </summary>
    internal sealed class RemoveNotificationsOnPersonalEventDateAndTimeChangedDomainEventHandler
        : IDomainEventHandler<PersonalEventDateAndTimeChangedDomainEvent>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IDateTime _dateTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveNotificationsOnPersonalEventDateAndTimeChangedDomainEventHandler"/> class.
        /// </summary>
        /// <param name="notificationRepository">The notification repository.</param>
        /// <param name="dateTime">The date and time.</param>
        public RemoveNotificationsOnPersonalEventDateAndTimeChangedDomainEventHandler(
            INotificationRepository notificationRepository,
            IDateTime dateTime)
        {
            _notificationRepository = notificationRepository;
            _dateTime = dateTime;
        }

        /// <inheritdoc />
        public async Task Handle(PersonalEventDateAndTimeChangedDomainEvent notification, CancellationToken cancellationToken) =>
            await _notificationRepository.RemoveNotificationsForEventAsync(notification.PersonalEvent, _dateTime.UtcNow);
    }
}
