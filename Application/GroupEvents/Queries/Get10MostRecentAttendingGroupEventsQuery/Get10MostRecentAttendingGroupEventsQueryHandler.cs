using Application.Core.Abstractions.Authentication;
using Application.Core.Abstractions.Data;
using Application.Core.Abstractions.Messaging;
using Contracts.GroupEvents;
using Domain.Core.Primitives.Maybe;
using Domain.Entities;
using Domain.Enumerations;
using Microsoft.EntityFrameworkCore;

namespace Application.GroupEvents.Queries.Get10MostRecentAttendingGroupEventsQuery;

internal sealed class Get10MostRecentAttendingGroupEventsQueryHandler
    : IQueryHandler<Get10MostRecentAttendingGroupEventsQuery, Maybe<IReadOnlyCollection<GroupEventResponse>>>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IDbContext _dbContext;

    public Get10MostRecentAttendingGroupEventsQueryHandler(IUserIdentifierProvider userIdentifierProvider, IDbContext dbContext)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _dbContext = dbContext;
    }

    public async Task<Maybe<IReadOnlyCollection<GroupEventResponse>>> Handle(Get10MostRecentAttendingGroupEventsQuery request, CancellationToken cancellationToken)
    {
        if (request.UserId == Guid.Empty || request.UserId != _userIdentifierProvider.UserId)
        {
            return Maybe<IReadOnlyCollection<GroupEventResponse>>.None;
        }

        GroupEventResponse[] responses = await(
            from attendee in _dbContext.Set<Attendee>().AsNoTracking()
            join groupEvent in _dbContext.Set<GroupEvent>().AsNoTracking()
                on attendee.EventId equals groupEvent.Id
            where attendee.UserId == request.UserId
            orderby groupEvent.DateTimeUtc
            select new GroupEventResponse
            {
                Id = groupEvent.Id,
                Name = groupEvent.Name.Value,
                CategoryId = groupEvent.Category.Value,
                DateTimeUtc = groupEvent.DateTimeUtc,
                CreatedOnUtc = groupEvent.CreatedOnUtc
            })
            .Take(request.NumberOfGroupEventsToTake)
            .ToArrayAsync(cancellationToken);

        foreach (GroupEventResponse groupEventResponse in responses)
        {
            groupEventResponse.Category = Category.FromValue(groupEventResponse.CategoryId).Value.Name;
        }

        return responses;
    }
}
