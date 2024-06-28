using Application.Core.Abstractions.Common;
using Application.Core.Abstractions.Data;
using Domain.Core.Primitives.Result;
using Domain.Entities;
using Domain.Enumerations;
using Domain.Repositories;

namespace BackgroundTasks.Services;

/// <summary>
/// Represents the group event notifications producer.
/// </summary>
internal sealed class GroupEventNotificationsProducer : IGroupEventNotificationsProducer
{
    private readonly IDateTime _dateTime;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAttendeeRepository _attendeeRepository;
    private readonly IGroupEventRepository _groupEventRepository;
    private readonly INotificationRepository _notificationRepository;

    public GroupEventNotificationsProducer(
        IDateTime dateTime,
        IUnitOfWork unitOfWork,
        IAttendeeRepository attendeeRepository,
        IGroupEventRepository groupEventRepository,
        INotificationRepository notificationRepository)
    {
        _dateTime = dateTime;
        _unitOfWork = unitOfWork;
        _attendeeRepository = attendeeRepository;
        _groupEventRepository = groupEventRepository;
        _notificationRepository = notificationRepository;
    }
    public async Task ProduceAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Attendee> unprocessedAttendees = await _attendeeRepository.GetUnprocessedAsync(batchSize);

        if (!unprocessedAttendees.Any())
        {
            return;
        }

        IReadOnlyCollection<GroupEvent> groupEvents = await _groupEventRepository.GetForAttendeesAsync(unprocessedAttendees);

        if(!groupEvents.Any())
        {
            return;
        }

        Dictionary<Guid, GroupEvent> groupEventsDictionary = groupEvents.ToDictionary(g => g.Id, g => g);

        var notifications = new List<Notification>();

        foreach(Attendee attendee in unprocessedAttendees)
        {
            Result result = attendee.MarkAsProcessed();

            if (result.IsFailure)
            {
                continue;
            }

            GroupEvent groupEvent = groupEventsDictionary[attendee.EventId];

            List<Notification> notificationsForAttendee = NotificationType
                .List
                .Select(notificationType => notificationType.TryCreateNotification(groupEvent, attendee.UserId, _dateTime.UtcNow))
                .Where(maybeNotification => maybeNotification.HasValue)
                .Select(maybeNotification => maybeNotification.Value)
                .ToList();

            notifications.AddRange(notificationsForAttendee);
        }

        _notificationRepository.InsertRange(notifications);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}