using Application.Core.Abstractions.Authentication;
using Application.Core.Abstractions.Data;
using Application.Core.Abstractions.Messaging;
using Contracts.Attendees;
using Domain.Core.Primitives.Maybe;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Attendees.Queries.GetAttendeesForEventId;

/// <summary>
/// Represents the <see cref="GetAttendeesForGroupEventIdQuery"/> handler.
/// </summary>
internal sealed class GetAttendeesForGroupEventIdQueryHandler : IQueryHandler<GetAttendeesForGroupEventIdQuery, Maybe<AttendeeListResponse>>
{
    private readonly IDbContext _dbContext;
    private readonly IUserIdentifierProvider _userIdentifierProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAttendeesForGroupEventIdQueryHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userIdentifierProvider">The user identifier provider.</param>
    public GetAttendeesForGroupEventIdQueryHandler(
        IDbContext dbContext,
        IUserIdentifierProvider userIdentifierProvider)
    {
        _dbContext = dbContext;
        _userIdentifierProvider = userIdentifierProvider;
    }

    public async Task<Maybe<AttendeeListResponse>> Handle(GetAttendeesForGroupEventIdQuery request, CancellationToken cancellationToken)
    {
        if (request.GroupEventId == Guid.Empty || !await HasPermissionsToQueryAttendees(request.GroupEventId))
        {
            return Maybe<AttendeeListResponse>.None;
        }

        AttendeeListResponse.AttendeeModel[] attendeeModels = await (
           from attendee in _dbContext.Set<Attendee>().AsNoTracking()
           join groupEvent in _dbContext.Set<GroupEvent>().AsNoTracking()
               on attendee.EventId equals groupEvent.Id
           join user in _dbContext.Set<User>().AsNoTracking()
               on attendee.UserId equals user.Id
           where groupEvent.Id == request.GroupEventId && !groupEvent.Cancelled
           select new AttendeeListResponse.AttendeeModel
           {
               UserId = attendee.UserId,
               Name = user.FirstName.Value + " " + user.LastName.Value,
               CreatedOnUtc = attendee.CreatedOnUtc
           }).ToArrayAsync(cancellationToken);

        // Select with methods
        //AttendeeListResponse.AttendeeModel[] attendeeModels = await _dbContext
        //        .Set<Attendee>()
        //        .AsNoTracking()
        //        .Join(_dbContext.Set<GroupEvent>().AsNoTracking(),
        //            attendee => attendee.EventId,
        //            groupEvent => groupEvent.Id,
        //            (attendee, groupEvent) => new { attendee, groupEvent })
        //        .Join(_dbContext.Set<User>().AsNoTracking(),
        //            combined => combined.attendee.UserId,
        //            user => user.Id,
        //            (combined, user) => new { combined.attendee, combined.groupEvent, user })
        //        .Where(x => x.groupEvent.Id == request.GroupEventId && !x.groupEvent.Cancelled)
        //        .Select(x => new AttendeeListResponse.AttendeeModel
        //        {
        //            UserId = x.attendee.UserId,
        //            Name = x.user.FirstName.Value + " " + x.user.LastName.Value,
        //            CreatedOnUtc = x.attendee.CreatedOnUtc
        //        })
        //        .ToArrayAsync(cancellationToken);

        var response = new AttendeeListResponse(attendeeModels);

        return response;
    }

    /// <summary>
    /// Checks if the current user has permissions to query attendees for this group event.
    /// </summary>
    /// <param name="groupEventId">The group event identifier.</param>
    /// <returns>True if the current user is the group event owner or an attendee.</returns>
    private async Task<bool> HasPermissionsToQueryAttendees(Guid groupEventId)
    {
        return await (
                from attendee in _dbContext.Set<Attendee>().AsNoTracking()
                join groupEvent in _dbContext.Set<GroupEvent>().AsNoTracking()
                    on attendee.EventId equals groupEvent.Id
                where groupEvent.Id == groupEventId &&
                      !groupEvent.Cancelled &&
                      (groupEvent.UserId == _userIdentifierProvider.UserId ||
                      attendee.UserId == _userIdentifierProvider.UserId)
                select true).AnyAsync();

        // Select with methods
        //var response = await _dbContext
        //       .Set<Attendee>()
        //       .AsNoTracking()
        //       .Join(_dbContext.Set<GroupEvent>().AsNoTracking(),
        //            attendee => attendee.EventId,
        //            groupEvent => groupEvent.Id,
        //            (attendee, groupEvent) => new { attendee, groupEvent })
        //       .Where(x => x.groupEvent.Id == groupEventId &&
        //             !x.groupEvent.Cancelled &&
        //             (x.groupEvent.UserId == _userIdentifierProvider.UserId ||
        //              x.attendee.UserId == _userIdentifierProvider.UserId))
        //       .Select(x => true)
        //       .AnyAsync()

    }

}
