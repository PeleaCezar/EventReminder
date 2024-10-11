using Application.Core.Abstractions.Authentication;
using Application.Core.Abstractions.Data;
using Application.Core.Abstractions.Messaging;
using Contracts.Invitations;
using Domain.Core.Primitives.Maybe;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Invitations.Queries.GetSentInvitations;

internal sealed class GetSentInvitationsQueryHandler
        : IQueryHandler<GetSentInvitationsQuery, Maybe<SentInvitationsListResponse>>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IDbContext _dbContext;

    public GetSentInvitationsQueryHandler(IUserIdentifierProvider userIdentifierProvider, IDbContext dbContext)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _dbContext = dbContext;
    }

    public async Task<Maybe<SentInvitationsListResponse>> Handle(
        GetSentInvitationsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.UserId == Guid.Empty || request.UserId != _userIdentifierProvider.UserId)
        {
            return Maybe<SentInvitationsListResponse>.None;
        }

        SentInvitationsListResponse.SentInvitationModel[] invitations = await (
            from invitation in _dbContext.Set<Invitation>().AsNoTracking()
            join friend in _dbContext.Set<User>().AsNoTracking()
                on invitation.UserId equals friend.Id
            join groupEvent in _dbContext.Set<GroupEvent>().AsNoTracking()
                on invitation.EventId equals groupEvent.Id
            join user in _dbContext.Set<User>().AsNoTracking()
                on groupEvent.UserId equals user.Id
            where user.Id == request.UserId &&
                  groupEvent.UserId == request.UserId &&
                  invitation.CompletedOnUtc == null
            select new SentInvitationsListResponse.SentInvitationModel
            {
                Id = invitation.Id,
                FriendId = friend.Id,
                FriendName = friend.FirstName.Value + " " + friend.LastName.Value,
                EventName = groupEvent.Name.Value,
                EventDateTimeUtc = groupEvent.DateTimeUtc,
                CreatedOnUtc = invitation.CreatedOnUtc
            }).ToArrayAsync(cancellationToken);

        var response = new SentInvitationsListResponse(invitations);

        return response;
    }
}
