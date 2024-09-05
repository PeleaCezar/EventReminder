using Application.Core.Abstractions.Messaging;
using Application.Core.Abstractions.Notifications;
using Application.GroupEvents.Events.GroupEventDateAndTimeChanged;
using BackgroundTasks.Abstractions.Messaging;
using Contracts.Emails;
using Domain.Core.Errors;
using Domain.Core.Exceptions;
using Domain.Core.Primitives.Maybe;
using Domain.Entities;
using Domain.Repositories;
using System.Globalization;

namespace BackgroundTasks.IntegrationEvents.GroupEvents.GroupEventDateAndTimeChanged
{
    internal sealed class NotifyAttendeesOnGroupEventDateAndTimeChangedIntegrationEventHandler : IIntegrationEventHandler<GroupEventDateAndTimeChangedIntegrationEvent>
    {
        private readonly IGroupEventRepository _groupEventRepository;
        private readonly IAttendeeRepository _attendeeRepository;
        private readonly IEmailNotificationService _emailNotificationService;

        /// <param name="groupEventRepository">The group event repository.</param>
        /// <param name="attendeeRepository">The attendee repository.</param>
        /// <param name="emailNotificationService">The email notification service.</param>
        public NotifyAttendeesOnGroupEventDateAndTimeChangedIntegrationEventHandler(
            IGroupEventRepository groupEventRepository,
            IAttendeeRepository attendeeRepository,
            IEmailNotificationService emailNotificationService)
        {
            _groupEventRepository = groupEventRepository;
            _attendeeRepository = attendeeRepository;
            _emailNotificationService = emailNotificationService;
        }

        public async Task Handle(GroupEventDateAndTimeChangedIntegrationEvent notification, CancellationToken cancellationToken)
        {
            Maybe<GroupEvent> maybeGroupEvent = await _groupEventRepository.GetByIdAsync(notification.GroupEventId);

            if (maybeGroupEvent.HasNoValue)
            {
                throw new DomainException(DomainErrors.GroupEvent.NotFound);
            }

            GroupEvent groupEvent = maybeGroupEvent.Value;

            (string Email, string Name)[] attendeeEmailsAndNames = await _attendeeRepository.GetEmailsAndNamesForGroupEvent(groupEvent);

            if (attendeeEmailsAndNames.Length == 0)
            {
                return;
            }

            IEnumerable<Task> sendGroupEventCancelledEmailTasks = attendeeEmailsAndNames
                .Select(emailAndName =>
                    new GroupEventDateAndTimeChangedEmail(
                        emailAndName.Email,
                        emailAndName.Name,
                        groupEvent.Name,
                        notification.PreviousDateAndTimeUtc.ToString(CultureInfo.InvariantCulture),
                        groupEvent.DateTimeUtc.ToString(CultureInfo.InvariantCulture)))
                .Select(_emailNotificationService.SendGroupEventDateAndTimeChangedEmail);

            await Task.WhenAll(sendGroupEventCancelledEmailTasks);
        }
    }
}
