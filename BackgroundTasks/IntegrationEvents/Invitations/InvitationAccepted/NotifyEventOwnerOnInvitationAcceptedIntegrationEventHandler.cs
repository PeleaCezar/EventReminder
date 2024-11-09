﻿using Application.Core.Abstractions.Notifications;
using Application.Invitations.Events.InvitationAccepted;
using BackgroundTasks.Abstractions.Messaging;
using Contracts.Emails;
using Domain.Core.Errors;
using Domain.Core.Exceptions;
using Domain.Core.Primitives.Maybe;
using Domain.Entities;
using Domain.Repositories;
using System.Globalization;

namespace BackgroundTasks.IntegrationEvents.Invitations.InvitationAccepted;

/// <summary>
/// Represents the <see cref="InvitationAcceptedIntegrationEvent"/> handler.
/// </summary>
internal sealed class NotifyEventOwnerOnInvitationAcceptedIntegrationEventHandler
    : IIntegrationEventHandler<InvitationAcceptedIntegrationEvent>
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IGroupEventRepository _groupEventRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmailNotificationService _emailNotificationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotifyEventOwnerOnInvitationAcceptedIntegrationEventHandler"/> class.
    /// </summary>
    /// <param name="invitationRepository">The invitation repository.</param>
    /// <param name="groupEventRepository">The group event repository.</param>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="emailNotificationService">The email notification service.</param>
    public NotifyEventOwnerOnInvitationAcceptedIntegrationEventHandler(
        IInvitationRepository invitationRepository,
        IGroupEventRepository groupEventRepository,
        IUserRepository userRepository,
        IEmailNotificationService emailNotificationService)
    {
        _invitationRepository = invitationRepository;
        _groupEventRepository = groupEventRepository;
        _userRepository = userRepository;
        _emailNotificationService = emailNotificationService;
    }

    public async Task Handle(InvitationAcceptedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        Maybe<Invitation> maybeInvitation = await _invitationRepository.GetByIdAsync(notification.InvitationId);

        if (maybeInvitation.HasNoValue)
        {
            throw new DomainException(DomainErrors.Invitation.NotFound);
        }

        Invitation invitation = maybeInvitation.Value;

        Maybe<GroupEvent> maybeGroupEvent = await _groupEventRepository.GetByIdAsync(invitation.EventId);

        if (maybeGroupEvent.HasNoValue)
        {
            throw new DomainException(DomainErrors.Invitation.EventNotFound);
        }

        GroupEvent groupEvent = maybeGroupEvent.Value;

        Maybe<User> maybeUser = await _userRepository.GetByIdAsync(groupEvent.UserId);

        if (maybeUser.HasNoValue)
        {
            throw new DomainException(DomainErrors.Invitation.UserNotFound);
        }

        Maybe<User> maybeFriend = await _userRepository.GetByIdAsync(invitation.UserId);

        if (maybeFriend.HasNoValue)
        {
            throw new DomainException(DomainErrors.Invitation.FriendNotFound);
        }

        User user = maybeUser.Value;

        User friend = maybeFriend.Value;

        var invitationAcceptedEmail = new InvitationAcceptedEmail(
            user.Email,
            user.FullName,
            friend.FullName,
            groupEvent.Name,
            groupEvent.DateTimeUtc.ToString(CultureInfo.InvariantCulture));

        await _emailNotificationService.SendInvitationAcceptedEmail(invitationAcceptedEmail);
    }
}
