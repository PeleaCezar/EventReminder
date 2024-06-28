using Application.Core.Abstractions.Authentication;
using Application.Core.Abstractions.Data;
using Application.Core.Abstractions.Messaging;
using Contracts.FriendshipRequests;
using Domain.Core.Primitives.Maybe;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.FriendshipRequests.Queries.GetFriendshipRequestById
{
    internal sealed class GetFriendshipRequestByIdQueryHandler
        : IQueryHandler<GetFriendshipRequestByIdQuery, Maybe<FriendshipRequestResponse>>
    {
        private readonly IUserIdentifierProvider _userIdentifierProvider;
        private readonly IDbContext _dbContext;

        public GetFriendshipRequestByIdQueryHandler(IUserIdentifierProvider userIdentifierProvider, IDbContext dbContext)
        {
            _userIdentifierProvider = userIdentifierProvider;
            _dbContext = dbContext;
        }

        public async Task<Maybe<FriendshipRequestResponse>> Handle(GetFriendshipRequestByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.FriendshipRequestId == Guid.Empty)
            {
                return Maybe<FriendshipRequestResponse>.None;
            }

            FriendshipRequestResponse response = await (
                from friendshipRequest in _dbContext.Set<FriendshipRequest>().AsNoTracking()
                join user in _dbContext.Set<User>().AsNoTracking()
                on friendshipRequest.UserId equals user.Id
                join friend in _dbContext.Set<User>().AsNoTracking()
                on friendshipRequest.FriendId equals friend.Id
                where friendshipRequest.Id == request.FriendshipRequestId && friendshipRequest.CompletedOnUtc == null
                select new FriendshipRequestResponse
                {
                    UserId = user.Id,
                    UserEmail = user.Email.Value,
                    UserName = user.FirstName.Value + " " + user.LastName.Value,
                    FriendId = friend.Id,
                    FriendEmail = friend.Email.Value,
                    FriendName = friend.FirstName.Value + " " + friend.LastName.Value,
                    CreatedOnUtc = friendshipRequest.CreatedOnUtc
                }).FirstOrDefaultAsync(cancellationToken);

            // Linq methods
            //FriendshipRequestResponse response = await _dbContext
            //    .Set<FriendshipRequest>()
            //    .AsNoTracking()
            //     .Join(_dbContext.Set<User>().AsNoTracking(),
            //        friendshipRequest => friendshipRequest.UserId,
            //        user => user.Id,
            //        (friendshipRequest, user) => new { friendshipRequest, user })
            //     .Join(_dbContext.Set<User>().AsNoTracking(),
            //        combined => combined.friendshipRequest.FriendId,
            //        friend => friend.Id,
            //         (combined, friend) => new { combined.friendshipRequest, combined.user, friend })
            //     .Where(x => x.friendshipRequest.Id == request.FriendshipRequestId && 
            //            x.friendshipRequest.CompletedOnUtc == null)
            //     .Select(y => new FriendshipRequestResponse
            //     {
            //         UserId = y.user.Id,
            //         UserEmail = y.user.Email.Value,
            //         UserName = y.user.FirstName.Value + " " + y.user.LastName.Value,
            //         FriendId = y.friend.Id,
            //         FriendEmail = y.friend.Email.Value,
            //         FriendName = y.friend.FirstName.Value + " " + y.friend.LastName.Value,
            //         CreatedOnUtc = y.friendshipRequest.CreatedOnUtc
            //     })
            //     .FirstOrDefaultAsync(cancellationToken);

            if (response is null)
            {
                return Maybe<FriendshipRequestResponse>.None;
            }

            if (response.UserId != _userIdentifierProvider.UserId || response.FriendId != _userIdentifierProvider.UserId)
            {
                return Maybe<FriendshipRequestResponse>.None;
            }

            return response;
        }
    }
}
