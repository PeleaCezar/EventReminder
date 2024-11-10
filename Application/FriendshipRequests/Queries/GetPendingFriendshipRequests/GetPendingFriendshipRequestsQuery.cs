using Application.Core.Abstractions.Messaging;
using Contracts.FriendshipRequests;
using Domain.Core.Primitives.Maybe;

namespace Application.FriendshipRequests.Queries.GetPendingFriendshipRequests;

/// <summary>
/// Represents the query for getting the pending friendship requests for the user identifier.
/// </summary>
public sealed class GetPendingFriendshipRequestsQuery : IQuery<Maybe<PendingFriendshipRequestsListResponse>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetPendingFriendshipRequestsQuery"/> class.
    /// </summary>
    /// <param name="userId">The user identifier provider.</param>
    public GetPendingFriendshipRequestsQuery(Guid userId) => UserId = userId;

    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    public Guid UserId { get; }
}