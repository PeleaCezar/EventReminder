using Application.Core.Abstractions.Authentication;
using Application.Core.Abstractions.Common;
using Application.Core.Abstractions.Data;
using Application.Core.Abstractions.Messaging;
using Domain.Core.Errors;
using Domain.Core.Primitives.Maybe;
using Domain.Core.Primitives.Result;
using Domain.Entities;
using Domain.Repositories;

namespace Application.FriendshipRequests.Commands;

/// <summary>
/// Represents the <see cref="AcceptFriendshipRequestCommand"/> handler.
/// </summary>
internal sealed class AcceptFriendshipRequestCommandHandler : ICommandHandler<AcceptFriendshipRequestCommand, Result>
{
    private readonly IFriendshipRequestRepository _friendshipRequestRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTime _dateTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="AcceptFriendshipRequestCommandHandler"/> class.
    /// </summary>
    /// <param name="userIdentifierProvider">The user identifier provider.</param>
    /// <param name="friendshipRequestRepository">The friendship request repository.</param>
    /// <param name="friendshipRepository">The friendship repository.</param>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="dateTime">The date and time.</param>
    public AcceptFriendshipRequestCommandHandler(
        IFriendshipRequestRepository friendshipRequestRepository,
        IUserIdentifierProvider userIdentifierProvider,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IDateTime dateTime)
    {
        _friendshipRequestRepository = friendshipRequestRepository;
        _userIdentifierProvider = userIdentifierProvider;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _dateTime = dateTime;
    }
    public async Task<Result> Handle(AcceptFriendshipRequestCommand request, CancellationToken cancellationToken)
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

        Maybe<User> maybeUser = await _userRepository.GetByIdAsync(friendshipRequest.UserId);

        if (maybeUser.HasNoValue)
        {
            return Result.Failure(DomainErrors.FriendshipRequest.UserNotFound);
        }

        Maybe<User> maybeFriend = await _userRepository.GetByIdAsync(friendshipRequest.FriendId);

        if (maybeFriend.HasNoValue)
        {
            return Result.Failure(DomainErrors.FriendshipRequest.FriendNotFound);
        }

        Result acceptResult = friendshipRequest.Accept(_dateTime.UtcNow);

        if (acceptResult.IsFailure)
        {
            return Result.Failure(acceptResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}