using Api.Contracts;
using Api.Infrastructure;
using Application.Users.Commands.ChangePassword;
using Application.Users.Commands.SendFriendshipRequest;
using Application.Users.Commands.UpdateUser;
using Application.Users.Query.GetUserById;
using Contracts.Users;
using Domain.Core.Errors;
using Domain.Core.Primitives.Maybe;
using Domain.Core.Primitives.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public sealed class UsersController : ApiController
{
    public UsersController(IMediator mediator) 
        : base(mediator)
    {
    }

    [HttpGet(ApiRoutes.Users.GetById)]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid userId) =>
        await Maybe<GetUserByIdQuery>
            .From(new GetUserByIdQuery(userId))
            .Bind(query => Mediator.Send(query))
            .Match(Ok, NotFound);

    [HttpPut(ApiRoutes.Users.Update)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid userId, UpdateUserRequest updateUserRequest) =>
        await Result.Create(updateUserRequest, DomainErrors.General.UnProcessableRequest)
            .Ensure(request => request.UserId == userId, DomainErrors.General.UnProcessableRequest)
            .Map(request => new UpdateUserCommand(request.UserId, request.FirstName, updateUserRequest.LastName))
            .Bind(command => Mediator.Send(command))
            .Match(Ok, BadRequest);

    [HttpPut(ApiRoutes.Users.ChangePassword)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword(Guid userId, ChangePasswordRequest changePasswordRequest) =>
    await Result.Create(changePasswordRequest, DomainErrors.General.UnProcessableRequest)
        .Ensure(request => request.UserId == userId, DomainErrors.General.UnProcessableRequest)
        .Map(request => new ChangePasswordCommand(request.UserId, request.Password))
        .Bind(command => Mediator.Send(command))
        .Match(Ok, BadRequest);

    [HttpPost(ApiRoutes.Users.SendFriendshipRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendFriendshipRequest(Guid userId, SendFriendshipRequestRequest sendFriendshipRequestRequest) =>
    await Result.Create(sendFriendshipRequestRequest, DomainErrors.General.UnProcessableRequest)
        .Ensure(request => request.UserId == userId, DomainErrors.General.UnProcessableRequest)
        .Map(request => new SendFriendshipRequestCommand(request.UserId, request.FriendId))
        .Bind(command => Mediator.Send(command))
        .Match(Ok, BadRequest);

}
