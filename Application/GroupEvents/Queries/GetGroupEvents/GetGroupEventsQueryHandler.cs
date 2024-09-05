using Application.Core.Abstractions.Authentication;
using Application.Core.Abstractions.Data;
using Application.Core.Abstractions.Messaging;
using Contracts.Common;
using Contracts.GroupEvents;
using Domain.Core.Primitives.Maybe;
using Domain.Entities;
using Domain.Enumerations;
using Microsoft.EntityFrameworkCore;

namespace Application.GroupEvents.Queries.GetGroupEvents;

internal sealed class GetGroupEventsQueryHandler : IQueryHandler<GetGroupEventsQuery, Maybe<PagedList<GroupEventResponse>>>
{
    private readonly IDbContext _dbContext;
    private readonly IUserIdentifierProvider _userIdentifierProvider;

    public GetGroupEventsQueryHandler(IDbContext dbContext, IUserIdentifierProvider userIdentifierProvider)
    {
        _dbContext = dbContext;
        _userIdentifierProvider = userIdentifierProvider;
    }
    public async Task<Maybe<PagedList<GroupEventResponse>>> Handle(GetGroupEventsQuery request, CancellationToken cancellationToken)
    {
        if(request.UserId == Guid.Empty || request.UserId != _userIdentifierProvider.UserId)
        {
            return Maybe<PagedList<GroupEventResponse>>.None;
        }

        bool shouldSearchCategory = request.CategoryId != null && Category.ContainsValue(request.CategoryId.Value);

        IQueryable<GroupEventResponse> groupEventResponsesQuery =
            from groupEvent in _dbContext.Set<GroupEvent>().AsNoTracking()
            join user in _dbContext.Set<User>().AsNoTracking()
                on groupEvent.UserId equals user.Id
            where groupEvent.UserId == request.UserId &&
                  !groupEvent.Cancelled &&
                  (!shouldSearchCategory || groupEvent.Category.Value == request.CategoryId) &&
                  (request.Name == null || request.Name == "" || groupEvent.Name.Value.Contains(request.Name)) &&
                  (request.StartDate == null || groupEvent.DateTimeUtc >= request.StartDate) &&
                  (request.EndDate == null || groupEvent.DateTimeUtc <= request.EndDate)
            orderby groupEvent.DateTimeUtc descending
            select new GroupEventResponse
            {
                Id = groupEvent.Id,
                Name = groupEvent.Name.Value,
                CategoryId = groupEvent.Category.Value,
                DateTimeUtc = groupEvent.DateTimeUtc,
                CreatedOnUtc = groupEvent.CreatedOnUtc
            };

        int totalCount = await groupEventResponsesQuery.CountAsync();

        GroupEventResponse[] groupEventResponsesPage = await groupEventResponsesQuery
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToArrayAsync(cancellationToken);

        foreach(GroupEventResponse groupEventResponse in groupEventResponsesPage)
        {
            groupEventResponse.Category = Category.FromValue(groupEventResponse.CategoryId).Value.Name;
        }

        return new PagedList<GroupEventResponse>(groupEventResponsesPage, request.Page, request.PageSize, totalCount);
    }
}
