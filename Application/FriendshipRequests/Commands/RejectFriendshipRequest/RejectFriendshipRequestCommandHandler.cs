﻿using Application.Core.Abstractions.Authentication;
using Application.Core.Abstractions.Common;
using Application.Core.Abstractions.Data;
using Application.Core.Abstractions.Messaging;
using Domain.Core.Errors;
using Domain.Core.Primitives.Maybe;
using Domain.Core.Primitives.Result;
using Domain.Entities;
using Domain.Repositories;

namespace Application.FriendshipRequests.Commands.RejectFriendshipRequest;

/// <summary>
/// Represents the <see cref="RejectFriendshipRequestCommand"/> handler.
/// </summary>
internal sealed class RejectFriendshipRequestCommandHandler : ICommandHandler<RejectFriendshipRequestCommand, Result>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IFriendshipRequestRepository _friendshipRequestRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTime _dateTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="RejectFriendshipRequestCommandHandler"/> class.
    /// </summary>
    /// <param name="userIdentifierProvider">The user identifier provider.</param>
    /// <param name="friendshipRequestRepository">The friendship request repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="dateTime">The date and time.</param>
    public RejectFriendshipRequestCommandHandler(
        IUserIdentifierProvider userIdentifierProvider,
        IFriendshipRequestRepository friendshipRequestRepository,
        IUnitOfWork unitOfWork,
        IDateTime dateTime)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _friendshipRequestRepository = friendshipRequestRepository;
        _unitOfWork = unitOfWork;
        _dateTime = dateTime;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(RejectFriendshipRequestCommand request, CancellationToken cancellationToken)
    {
        Maybe<FriendshipRequest> maybeFriendshipRequest = await _friendshipRequestRepository.GetByIdAsync(request.FriendshipRequestId);

        if (maybeFriendshipRequest.HasNoValue)
        {
            return Result.Failure(DomainErrors.FriendshipRequest.NotFound);
        }

        FriendshipRequest friendshipRequest = maybeFriendshipRequest.Value;

        if (friendshipRequest.FriendId != _userIdentifierProvider.UserId)
        {
            return Result.Failure(DomainErrors.User.InvalidPermissions);
        }

        Result rejectResult = friendshipRequest.Reject(_dateTime.UtcNow);

        if (rejectResult.IsFailure)
        {
            return Result.Failure(rejectResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
