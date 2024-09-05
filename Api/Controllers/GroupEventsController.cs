using Api.Contracts;
using Api.Infrastructure;
using Application.GroupEvents.Queries.Get10MostRecentAttendingGroupEventsQuery;
using Application.GroupEvents.Queries.GetGroupEventById;
using Application.GroupEvents.Queries.GetGroupEvents;
using Contracts.Common;
using Contracts.GroupEvents;
using Domain.Core.Primitives.Maybe;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;


public sealed class GroupEventsController : ApiController
{
    public GroupEventsController(IMediator mediator) 
        : base(mediator)
    {    
    }

    [HttpGet(ApiRoutes.GroupEvents.GetById)]
    [ProducesResponseType(typeof(DetailedGroupEventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid groupEventId) =>
        await Maybe<GetGroupEventByIdQuery>
            .From(new GetGroupEventByIdQuery(groupEventId))
            .Bind(query => Mediator.Send(query))
            .Match(Ok, NotFound);

    [HttpGet(ApiRoutes.GroupEvents.Get)]
    [ProducesResponseType(typeof(PagedList<GroupEventResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(
        Guid userId,
        string name,
        int? categoryId,
        DateTime? startDate,
        DateTime? endDate,
        int page,
        int pageSize) =>
        await Maybe<GetGroupEventsQuery>
        .From(new GetGroupEventsQuery(userId, name, categoryId, startDate, endDate, page, pageSize))
        .Bind(query => Mediator.Send(query))
        .Match(Ok, NotFound);

    [HttpGet(ApiRoutes.GroupEvents.GetMostRecentAttending)]
    [ProducesResponseType(typeof(IReadOnlyCollection<GroupEventResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMostRecentAttending(Guid userId) =>
        await Maybe<Get10MostRecentAttendingGroupEventsQuery>
             .From(new Get10MostRecentAttendingGroupEventsQuery(userId))
             .Bind(query => Mediator.Send(query))
             .Match(Ok, NotFound);
}
