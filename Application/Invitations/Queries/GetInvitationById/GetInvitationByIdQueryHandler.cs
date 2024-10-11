using Application.Core.Abstractions.Authentication;
using Application.Core.Abstractions.Data;
using Application.Core.Abstractions.Messaging;
using Contracts.Invitations;
using Domain.Core.Primitives.Maybe;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Invitations.Queries.GetInvitationById;

internal sealed class GetInvitationByIdQueryHandler : IQueryHandler<GetInvitationByIdQuery, Maybe<InvitationResponse>>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IDbContext _dbContext;

    public GetInvitationByIdQueryHandler(IUserIdentifierProvider userIdentifierProvider, IDbContext dbContext)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _dbContext = dbContext;
    }

    public async Task<Maybe<InvitationResponse>> Handle(GetInvitationByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.InvitationId == Guid.Empty)
        {
            return Maybe<InvitationResponse>.None;
        }

        InvitationResponse response = await (
            from invitation in _dbContext.Set<Invitation>().AsNoTracking()
            join user in _dbContext.Set<User>().AsNoTracking()
                on invitation.UserId equals user.Id
            join groupEvent in _dbContext.Set<GroupEvent>().AsNoTracking()
                on invitation.EventId equals groupEvent.Id
            join friend in _dbContext.Set<User>().AsNoTracking()
                on groupEvent.UserId equals friend.Id
            where invitation.Id == request.InvitationId &&
                  invitation.UserId == _userIdentifierProvider.UserId &&
                  invitation.CompletedOnUtc == null
            select new InvitationResponse
            {
                Id = invitation.Id,
                EventName = groupEvent.Name.Value,
                EventDateTimeUtc = groupEvent.DateTimeUtc,
                FriendName = friend.FirstName.Value + " " + friend.LastName.Value,
                CreatedOnUtc = invitation.CreatedOnUtc
            }).FirstOrDefaultAsync(cancellationToken);

        return response;
    }
}
