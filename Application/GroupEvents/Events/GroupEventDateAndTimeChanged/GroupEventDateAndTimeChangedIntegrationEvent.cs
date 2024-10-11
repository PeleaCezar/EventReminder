using Application.Core.Abstractions.Messaging;
using Domain.Events;
using Newtonsoft.Json;

namespace Application.GroupEvents.Events.GroupEventDateAndTimeChanged;

public sealed class GroupEventDateAndTimeChangedIntegrationEvent : IIntegrationEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupEventDateAndTimeChangedIntegrationEvent"/> class.
    /// </summary>
    /// <param name="groupEventDateAndTimeChangedDomainEvent">The group event date and time changed domain event.</param>
    internal GroupEventDateAndTimeChangedIntegrationEvent(
        GroupEventDateAndTimeChangedDomainEvent groupEventDateAndTimeChangedDomainEvent)
    {
        GroupEventId = groupEventDateAndTimeChangedDomainEvent.GroupEvent.Id;
        PreviousDateAndTimeUtc = groupEventDateAndTimeChangedDomainEvent.PreviousDateAndTimeUtc;
    }

    [JsonConstructor]
    private GroupEventDateAndTimeChangedIntegrationEvent(Guid groupEventId, DateTime previousDateAndTimeUtc)
    {
        GroupEventId = groupEventId;
        PreviousDateAndTimeUtc = previousDateAndTimeUtc;
    }

    /// <summary>
    /// Gets the group event identifier.
    /// </summary>
    public Guid GroupEventId { get; }

    /// <summary>
    /// Gets the previous group event date and time in UTC format.
    /// </summary>
    public DateTime PreviousDateAndTimeUtc { get; }
}
