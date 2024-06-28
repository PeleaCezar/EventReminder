using Application.Core.Abstractions.Messaging;
using Contracts.FriendshipRequests;
using Domain.Core.Primitives.Maybe;

namespace Application.FriendshipRequests.Queries.GetFriendshipRequestById
{
    /// <summary>
    /// Represents the query for getting the friendship request by the identifier.
    /// </summary>
    public sealed class GetFriendshipRequestByIdQuery : IQuery<Maybe<FriendshipRequestResponse>>
    {
        public GetFriendshipRequestByIdQuery(Guid friendshipRequestId) => FriendshipRequestId = friendshipRequestId;

        public Guid FriendshipRequestId { get; }
    }
}
