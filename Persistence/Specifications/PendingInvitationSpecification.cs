using Domain.Entities;
using System.Linq.Expressions;

namespace Persistence.Specifications;

internal sealed class PendingInvitationSpecification : Specification<Invitation>
{
    private readonly Guid _groupEventId;
    private readonly Guid _userId;

    internal PendingInvitationSpecification(GroupEvent groupEvent, User user)
    {
        _groupEventId = groupEvent.Id;
        _userId = user.Id;
    }

    internal override Expression<Func<Invitation, bool>> ToExpression() =>
        invitation => invitation.CompletedOnUtc == null &&
                      invitation.EventId == _groupEventId &&
                      invitation.UserId == _userId;
}
