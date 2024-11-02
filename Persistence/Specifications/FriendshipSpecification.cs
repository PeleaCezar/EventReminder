using Domain.Entities;
using System.Linq.Expressions;

namespace Persistence.Specifications;

internal sealed class FriendshipSpecification : Specification<Friendship>
{
    private readonly Guid _userId;
    private readonly Guid _friendId;

    public FriendshipSpecification(User user, User friend)
    {
        _userId= user.Id;
        _friendId= friend.Id;
    }

    internal override Expression<Func<Friendship, bool>> ToExpression() =>
        friendship => friendship.UserId == _userId && friendship.FriendId == _friendId;
}
