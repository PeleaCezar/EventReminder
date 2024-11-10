using Application.Core.Abstractions.Messaging;
using Domain.Core.Primitives.Result;

namespace Application.FriendshipRequests.Commands.RejectFriendshipRequest;

/// <summary>
/// Represents the reject friendship request command.
/// </summary>
public sealed class RejectFriendshipRequestCommand : ICommand<Result>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RejectFriendshipRequestCommand"/> class.
    /// </summary>
    /// <param name="friendshipRequestId">The friendship request identifier.</param>
    public RejectFriendshipRequestCommand(Guid friendshipRequestId) => FriendshipRequestId = friendshipRequestId;

    /// <summary>
    /// Gets the friendship request identifier.
    /// </summary>
    public Guid FriendshipRequestId { get; }
}