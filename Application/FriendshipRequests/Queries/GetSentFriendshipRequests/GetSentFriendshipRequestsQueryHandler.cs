using Application.Core.Abstractions.Authentication;
using Application.Core.Abstractions.Data;
using Application.Core.Abstractions.Messaging;
using Contracts.FriendshipRequests;
using Domain.Core.Primitives.Maybe;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.FriendshipRequests.Queries.GetSentFriendshipRequests;

internal sealed class GetSentFriendshipRequestsQueryHandler
    : IQueryHandler<GetSentFriendshipRequestsQuery, Maybe<SentFriendshipRequestsListResponse>>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IDbContext _dbContext;

    public GetSentFriendshipRequestsQueryHandler(IUserIdentifierProvider userIdentifierProvider, IDbContext dbContext)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _dbContext = dbContext;
    }

    public async Task<Maybe<SentFriendshipRequestsListResponse>> Handle(GetSentFriendshipRequestsQuery request, CancellationToken cancellationToken)
    {
        if (request.UserId == Guid.Empty || request.UserId != _userIdentifierProvider.UserId)
        {
            return Maybe<SentFriendshipRequestsListResponse>.None;
        }

        SentFriendshipRequestsListResponse.SentFriendshipRequestModel[] friendshipRequests = await (
                from friendshipRequest in _dbContext.Set<FriendshipRequest>().AsNoTracking()
                join user in _dbContext.Set<User>().AsNoTracking()
                    on friendshipRequest.FriendId equals user.Id
                where friendshipRequest.UserId == request.UserId && friendshipRequest.CompletedOnUtc == null
                select new SentFriendshipRequestsListResponse.SentFriendshipRequestModel
                {
                    Id = friendshipRequest.Id,
                    FriendId = user.Id,
                    FriendName = user.FirstName.Value + " " + user.LastName.Value,
                    CreatedOnUtc = friendshipRequest.CreatedOnUtc
                }).ToArrayAsync(cancellationToken);

        var response = new SentFriendshipRequestsListResponse(friendshipRequests);

        return response;
    }
}
