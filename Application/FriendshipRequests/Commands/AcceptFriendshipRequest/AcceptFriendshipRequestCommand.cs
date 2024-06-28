using Application.Core.Abstractions.Messaging;
using Domain.Core.Primitives.Result;

namespace Application.FriendshipRequests.Commands;

public sealed class AcceptFriendshipRequestCommand : ICommand<Result>
{
    public AcceptFriendshipRequestCommand(Guid friendshipRequestId) => FriendshipRequestId = friendshipRequestId;

    public Guid FriendshipRequestId { get; }
}
