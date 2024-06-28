using Application.Core.Abstractions.Messaging;
using Domain.Core.Primitives.Result;

namespace Application.FriendshipRequests.Commands.RejectFriendshipRequest;

public sealed class RejectFriendshipRequestCommand : ICommand<Result>
{
    public RejectFriendshipRequestCommand(Guid friendshipRequestId) => FriendshipRequestId = friendshipRequestId;

    public Guid FriendshipRequestId { get; }
}