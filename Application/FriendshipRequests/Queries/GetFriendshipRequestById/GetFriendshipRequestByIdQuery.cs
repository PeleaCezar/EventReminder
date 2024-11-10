using Application.Core.Abstractions.Messaging;
using Contracts.FriendshipRequests;
using Domain.Core.Primitives.Maybe;

namespace Application.FriendshipRequests.Queries.GetFriendshipRequestById;

/// <summary>
/// Represents the query for getting the friendship request by the identifier.
/// </summary>
public sealed class GetFriendshipRequestByIdQuery : IQuery<Maybe<FriendshipRequestResponse>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetFriendshipRequestByIdQuery"/> class.
    /// </summary>
    /// <param name="friendshipRequestId">The friendship request identifier.</param>
    public GetFriendshipRequestByIdQuery(Guid friendshipRequestId) => FriendshipRequestId = friendshipRequestId;

    /// <summary>
    /// Gets the friendship request identifier.
    /// </summary>
    public Guid FriendshipRequestId { get; }
}
