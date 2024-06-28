using Api.Contracts;
using Api.Infrastructure;
using Application.FriendshipRequests.Commands;
using Application.FriendshipRequests.Commands.RejectFriendshipRequest;
using Application.FriendshipRequests.Queries.GetFriendshipRequestById;
using Application.FriendshipRequests.Queries.GetPendingFriendshipRequests;
using Application.FriendshipRequests.Queries.GetSentFriendshipRequests;
using Contracts.FriendshipRequests;
using Domain.Core.Primitives.Maybe;
using Domain.Core.Primitives.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public sealed class FriendshipRequestsController : ApiController
    {
        public FriendshipRequestsController(IMediator mediator) 
            : base(mediator)
        {
        }

        [HttpGet(ApiRoutes.FriendshipRequests.GetById)]
        [ProducesResponseType(typeof(FriendshipRequestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid friendshipRequestId) =>
            await Maybe<GetFriendshipRequestByIdQuery>
                .From(new GetFriendshipRequestByIdQuery(friendshipRequestId))
                .Bind(query => Mediator.Send(query))
                .Match(Ok, NotFound);

        [HttpGet(ApiRoutes.FriendshipRequests.GetPending)]
        [ProducesResponseType(typeof(PendingFriendshipRequestsListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPending(Guid userId) =>
         await Maybe<GetPendingFriendshipRequestsQuery>
             .From(new GetPendingFriendshipRequestsQuery(userId))
             .Bind(query => Mediator.Send(query))
             .Match(Ok, NotFound);

        [HttpGet(ApiRoutes.FriendshipRequests.GetSent)]
        [ProducesResponseType(typeof(SentFriendshipRequestsListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult>GetSent(Guid userId) =>
            await Maybe<GetSentFriendshipRequestsQuery>
                 .From(new GetSentFriendshipRequestsQuery(userId))
                 .Bind(query => Mediator.Send(query))
                 .Match(Ok, NotFound);

        [HttpPost(ApiRoutes.FriendshipRequests.Accept)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Accept(Guid friendshipRequestId) =>
              await Result.Success(new AcceptFriendshipRequestCommand(friendshipRequestId))
                  .Bind(command => Mediator.Send(command))
                  .Match(Ok, BadRequest);

        [HttpPost(ApiRoutes.FriendshipRequests.Reject)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Reject(Guid friendshipRequestId) =>
            await Result.Success(new RejectFriendshipRequestCommand(friendshipRequestId))
                .Bind(command => Mediator.Send(command))
                .Match(Ok, BadRequest);
    }
}
