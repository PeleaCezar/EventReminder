using Application.Core.Abstractions.Messaging;

namespace Application.Attendees.Events.AttendeeCreated;

public sealed class AttendeeCreatedIntegrationEvent : IIntegrationEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AttendeeCreatedIntegrationEvent"/> class.
    /// </summary>
    /// <param name="attendeeCreatedEvent">The attendee created event.</param>
    internal AttendeeCreatedIntegrationEvent(AttendeeCreatedEvent attendeeCreatedEvent) =>
        AttendeeId = attendeeCreatedEvent.AttendeeId;

    /// <summary>
    /// Gets the attendee identifier.
    /// </summary>
    public Guid AttendeeId { get; }
}
