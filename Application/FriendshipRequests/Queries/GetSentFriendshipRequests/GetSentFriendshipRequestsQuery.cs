using Application.Core.Abstractions.Messaging;
using Contracts.FriendshipRequests;
using Domain.Core.Primitives.Maybe;

namespace Application.FriendshipRequests.Queries.GetSentFriendshipRequests;

public sealed class GetSentFriendshipRequestsQuery : IQuery<Maybe<SentFriendshipRequestsListResponse>>
{
    public GetSentFriendshipRequestsQuery(Guid userId) => UserId = userId;

    public Guid UserId { get; }
}
