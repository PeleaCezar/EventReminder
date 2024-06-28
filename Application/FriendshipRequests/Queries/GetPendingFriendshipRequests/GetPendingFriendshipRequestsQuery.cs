using Application.Core.Abstractions.Messaging;
using Contracts.FriendshipRequests;
using Domain.Core.Primitives.Maybe;

namespace Application.FriendshipRequests.Queries.GetPendingFriendshipRequests
{
    /// <summary>
    /// Represents the query for getting the pending friendship requests for the user identifier.
    /// </summary>
    public sealed class GetPendingFriendshipRequestsQuery : IQuery<Maybe<PendingFriendshipRequestsListResponse>>
    {
        public GetPendingFriendshipRequestsQuery(Guid userId) => UserId = userId;

        public Guid UserId { get; }
    }
}
