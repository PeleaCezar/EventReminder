using Application.Core.Abstractions.Messaging;
using Domain.Events;
using Newtonsoft.Json;

namespace Application.FriendshipRequests.Events.FriendshipRequestAccepted;

/// <summary>
/// Represents the integration event that is raised when a friendship request is accepted.
/// </summary>
public sealed class FriendshipRequestAcceptedIntegrationEvent : IIntegrationEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FriendshipRequestAcceptedIntegrationEvent"/> class.
    /// </summary>
    /// <param name="friendshipRequestAcceptedDomainEvent">The friendship request accepted domain event.</param>
    internal FriendshipRequestAcceptedIntegrationEvent(FriendshipRequestAcceptedDomainEvent friendshipRequestAcceptedDomainEvent) =>
    FriendshipRequestId = friendshipRequestAcceptedDomainEvent.FriendshipRequest.Id;

    [JsonConstructor]
    private FriendshipRequestAcceptedIntegrationEvent(Guid friendshipRequestId) => FriendshipRequestId = friendshipRequestId;

    public Guid FriendshipRequestId { get; }
}
