using Application.Core.Abstractions.Notifications;
using Application.GroupEvents.Events.GroupEventNameChanged;
using BackgroundTasks.Abstractions.Messaging;
using Contracts.Emails;
using Domain.Core.Errors;
using Domain.Core.Exceptions;
using Domain.Core.Primitives.Maybe;
using Domain.Entities;
using Domain.Repositories;
using System.Globalization;

namespace BackgroundTasks.IntegrationEvents.GroupEvents.GroupEventNameChanged;

internal sealed class NotifyAttendeesOnGroupEventNameChangedIntegrationEventHandler
        : IIntegrationEventHandler<GroupEventNameChangedIntegrationEvent>
{
    private readonly IGroupEventRepository _groupEventRepository;
    private readonly IAttendeeRepository _attendeeRepository;
    private readonly IEmailNotificationService _emailNotificationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotifyAttendeesOnGroupEventNameChangedIntegrationEventHandler"/> class.
    /// </summary>
    /// <param name="groupEventRepository">The group event repository.</param>
    /// <param name="attendeeRepository">The attendee repository.</param>
    /// <param name="emailNotificationService">The email notification service.</param>
    public NotifyAttendeesOnGroupEventNameChangedIntegrationEventHandler(
        IGroupEventRepository groupEventRepository,
        IAttendeeRepository attendeeRepository,
        IEmailNotificationService emailNotificationService)
    {
        _groupEventRepository = groupEventRepository;
        _attendeeRepository = attendeeRepository;
        _emailNotificationService = emailNotificationService;
    }

    /// <inheritdoc />
    public async Task Handle(GroupEventNameChangedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        Maybe<GroupEvent> maybeGroupEvent = await _groupEventRepository.GetByIdAsync(notification.GroupEventId);

        if (maybeGroupEvent.HasNoValue)
        {
            throw new DomainException(DomainErrors.GroupEvent.NotFound);
        }

        GroupEvent groupEvent = maybeGroupEvent.Value;

        (string Email, string Name)[] attendeeEmailsAndNames = await _attendeeRepository
            .GetEmailsAndNamesForGroupEvent(groupEvent);

        if (attendeeEmailsAndNames.Length == 0)
        {
            return;
        }

        IEnumerable<Task> sendGroupEventCancelledEmailTasks = attendeeEmailsAndNames
            .Select(emailAndName =>
                new GroupEventNameChangedEmail(
                    emailAndName.Email,
                    emailAndName.Name,
                    groupEvent.Name,
                    notification.PreviousName,
                    groupEvent.DateTimeUtc.ToString(CultureInfo.InvariantCulture)))
            .Select(groupEventNameChangedEmail => _emailNotificationService.SendGroupEventNameChangedEmail(groupEventNameChangedEmail));

        await Task.WhenAll(sendGroupEventCancelledEmailTasks);
    }
}
