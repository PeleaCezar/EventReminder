using Domain.Entities;
using System.Linq.Expressions;

namespace Persistence.Specifications;

internal sealed class PendingFriendshipRequestSpecification : Specification<FriendshipRequest>
{
    private readonly Guid _userId;
    private readonly Guid _friendId;

    internal PendingFriendshipRequestSpecification(User user, User friend)
    {
        _userId = user.Id;
        _friendId = friend.Id;
    }

    internal override Expression<Func<FriendshipRequest, bool>> ToExpression() =>
        friendshipRequest => (friendshipRequest.UserId == _userId || friendshipRequest.UserId == _friendId) &&
                             (friendshipRequest.FriendId == _userId || friendshipRequest.FriendId == _friendId) &&
                             friendshipRequest.CompletedOnUtc == null;
}
