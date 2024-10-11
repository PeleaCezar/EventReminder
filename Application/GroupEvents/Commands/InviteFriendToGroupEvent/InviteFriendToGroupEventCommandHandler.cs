﻿using Application.Core.Abstractions.Authentication;
using Application.Core.Abstractions.Data;
using Application.Core.Abstractions.Messaging;
using Domain.Core.Errors;
using Domain.Core.Primitives.Maybe;
using Domain.Core.Primitives.Result;
using Domain.Entities;
using Domain.Repositories;

namespace Application.GroupEvents.Commands.InviteFriendToGroupEvent;

/// <summary>
/// Represents the <see cref="InviteFriendToGroupEventCommand"/> handler.
/// </summary>
internal sealed class InviteFriendToGroupEventCommandHandler : ICommandHandler<InviteFriendToGroupEventCommand, Result>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IGroupEventRepository _groupEventRepository;
    private readonly IInvitationRepository _invitationRepository;
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="InviteFriendToGroupEventCommandHandler"/> class.
    /// </summary>
    /// <param name="userIdentifierProvider">The user identifier provider.</param>
    /// <param name="groupEventRepository">The group event repository.</param>
    /// <param name="invitationRepository">The invitation repository.</param>
    /// <param name="friendshipRepository">The friendship repository.</param>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public InviteFriendToGroupEventCommandHandler(
        IUserIdentifierProvider userIdentifierProvider,
        IGroupEventRepository groupEventRepository,
        IInvitationRepository invitationRepository,
        IFriendshipRepository friendshipRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _groupEventRepository = groupEventRepository;
        _invitationRepository = invitationRepository;
        _friendshipRepository = friendshipRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(InviteFriendToGroupEventCommand request, CancellationToken cancellationToken)
    {
        Maybe<GroupEvent> maybeGroupEvent = await _groupEventRepository.GetByIdAsync(request.GroupEventId);

        if(maybeGroupEvent.HasNoValue)
        {
            return Result.Failure(DomainErrors.GroupEvent.NotFound);
        }

        GroupEvent groupEvent = maybeGroupEvent.Value;

        if(groupEvent.UserId != _userIdentifierProvider.UserId)
        {
            return Result.Failure(DomainErrors.User.InvalidPermissions);
        }

        Maybe<User> maybeUser = await _userRepository.GetByIdAsync(groupEvent.UserId);

        if(maybeUser.HasNoValue)
        {
            return Result.Failure(DomainErrors.GroupEvent.UserNotFound);
        }

        Maybe<User> maybeFriend = await _userRepository.GetByIdAsync(request.FriendId);

        if(maybeFriend.HasNoValue)
        {
            return Result.Failure(DomainErrors.GroupEvent.FriendNotFound);
        }

        User user = maybeUser.Value;

        User friend = maybeFriend.Value;

        if(!await _friendshipRepository.CheckIfFriendsAsync(user, friend))
        {
            return Result.Failure(DomainErrors.GroupEvent.NotFriends);
        }

        Result<Invitation> invitationResult = await groupEvent.InviteAsync(friend, _invitationRepository);

        if(invitationResult.IsFailure)
        {
            return Result.Failure(invitationResult.Error);
        }

        Invitation invitation = invitationResult.Value;

        _invitationRepository.Insert(invitation);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
