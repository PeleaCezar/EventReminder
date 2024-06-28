using Api.Contracts;
using Api.Infrastructure;
using Application.Attendees.Queries.GetAttendeesForEventId;
using Contracts.Attendees;
using Domain.Core.Primitives.Maybe;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public sealed class AttendeesController : ApiController
{
    public AttendeesController(IMediator mediator) 
        : base(mediator)
    {
    }

    [HttpGet(ApiRoutes.Attendees.Get)]
    [ProducesResponseType(typeof(AttendeeListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid groupEventId) =>
        await Maybe<GetAttendeesForGroupEventIdQuery>
            .From(new GetAttendeesForGroupEventIdQuery(groupEventId))
            .Bind(query => Mediator.Send(query))
            .Match(Ok, NotFound);
}
