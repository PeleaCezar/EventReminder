using Application.Core.Abstractions.Authentication;
using Application.Core.Abstractions.Data;
using Application.Core.Abstractions.Messaging;
using Contracts.FriendshipRequests;
using Domain.Core.Primitives.Maybe;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.FriendshipRequests.Queries.GetPendingFriendshipRequests
{
    /// <summary>
    /// Represents the <see cref="GetPendingFriendshipRequestsQuery"/> handler.
    /// </summary>
    internal sealed class GetPendingFriendshipRequestsQueryHandler
        : IQueryHandler<GetPendingFriendshipRequestsQuery, Maybe<PendingFriendshipRequestsListResponse>>
    {
        private readonly IUserIdentifierProvider _userIdentifierProvider;
        private readonly IDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetPendingFriendshipRequestsQueryHandler"/> class.
        /// </summary>
        /// <param name="userIdentifierProvider">The user identifier provider.</param>
        /// <param name="dbContext">The database context.</param>
        public GetPendingFriendshipRequestsQueryHandler(IUserIdentifierProvider userIdentifierProvider, IDbContext dbContext)
        {
            _userIdentifierProvider = userIdentifierProvider;
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<Maybe<PendingFriendshipRequestsListResponse>> Handle(
            GetPendingFriendshipRequestsQuery request,
            CancellationToken cancellationToken)
        {
            if (request.UserId == Guid.Empty || request.UserId != _userIdentifierProvider.UserId)
            {
                return Maybe<PendingFriendshipRequestsListResponse>.None;
            }

            PendingFriendshipRequestsListResponse.PendingFriendshipRequestModel[] friendshipRequests = await (
                    from friendshipRequest in _dbContext.Set<FriendshipRequest>().AsNoTracking()
                    join user in _dbContext.Set<User>().AsNoTracking()
                        on friendshipRequest.UserId equals user.Id
                    where friendshipRequest.FriendId == request.UserId && friendshipRequest.CompletedOnUtc == null
                    select new PendingFriendshipRequestsListResponse.PendingFriendshipRequestModel
                    {
                        Id = friendshipRequest.Id,
                        FriendId = user.Id,
                        FriendName = user.FirstName.Value + " " + user.LastName.Value,
                        CreatedOnUtc = friendshipRequest.CreatedOnUtc
                    }).ToArrayAsync(cancellationToken);

            var response = new PendingFriendshipRequestsListResponse(friendshipRequests);

            return response;
        }
    }
}
