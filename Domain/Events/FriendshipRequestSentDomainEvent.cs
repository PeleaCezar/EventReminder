using Domain.Core.Events;
using Domain.Entities;

namespace Domain.Events;

/// <summary>
/// Represents the event that is raised when a friendship request is sent.
/// </summary>
public sealed class FriendshipRequestSentDomainEvent : IDomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FriendshipRequestSentDomainEvent"/> class.
    /// </summary>
    /// <param name="friendshipRequest">The friendship request.</param>
    internal FriendshipRequestSentDomainEvent(FriendshipRequest friendshipRequest) => FriendshipRequest = friendshipRequest;

    /// <summary>
    /// Gets the friendship request.
    /// </summary>
    public FriendshipRequest FriendshipRequest { get; }
}
