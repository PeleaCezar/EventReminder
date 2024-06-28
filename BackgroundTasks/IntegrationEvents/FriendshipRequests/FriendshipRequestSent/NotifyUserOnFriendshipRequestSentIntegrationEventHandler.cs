using Application.Core.Abstractions.Notifications;
using Application.FriendshipRequests.Events.FriendshipRequestSent;
using BackgroundTasks.Abstractions.Messaging;
using Contracts.Emails;
using Domain.Core.Errors;
using Domain.Core.Exceptions;
using Domain.Core.Primitives.Maybe;
using Domain.Entities;
using Domain.Repositories;


namespace BackgroundTasks.IntegrationEvents.FriendshipRequests.FriendshipRequestSent;

internal sealed class NotifyUserOnFriendshipRequestSentIntegrationEventHandler
    : IIntegrationEventHandler<FriendshipRequestSentIntegrationEvent>
{
    private readonly IFriendshipRequestRepository _friendshipRequestRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmailNotificationService _emailNotificationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotifyUserOnFriendshipRequestSentIntegrationEventHandler"/> class.
    /// </summary>
    /// <param name="friendshipRequestRepository">The friendship request repository.</param>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="emailNotificationService">The email notification service.</param>
    public NotifyUserOnFriendshipRequestSentIntegrationEventHandler(
        IFriendshipRequestRepository friendshipRequestRepository,
        IUserRepository userRepository,
        IEmailNotificationService emailNotificationService)
    {
        _friendshipRequestRepository = friendshipRequestRepository;
        _userRepository = userRepository;
        _emailNotificationService = emailNotificationService;
    }

    public async Task Handle(FriendshipRequestSentIntegrationEvent notification, CancellationToken cancellationToken)
    {
        Maybe<FriendshipRequest> maybeFriendshipRequest = await _friendshipRequestRepository
            .GetByIdAsync(notification.FriendshipRequestId);

        if (maybeFriendshipRequest.HasNoValue)
        {
            throw new DomainException(DomainErrors.FriendshipRequest.NotFound);
        }

        FriendshipRequest friendshipRequest = maybeFriendshipRequest.Value;

        Maybe<User> maybeUser = await _userRepository.GetByIdAsync(friendshipRequest.UserId);

        if (maybeUser.HasNoValue)
        {
            throw new DomainException(DomainErrors.FriendshipRequest.UserNotFound);
        }

        Maybe<User> maybeFriend = await _userRepository.GetByIdAsync(friendshipRequest.FriendId);

        if (maybeFriend.HasNoValue)
        {
            throw new DomainException(DomainErrors.FriendshipRequest.FriendNotFound);
        }

        User user = maybeUser.Value;

        User friend = maybeFriend.Value;

        var friendshipRequestSentEmail = new FriendshipRequestSentEmail(friend.Email, friend.FullName, user.FullName);

        await _emailNotificationService.SendFriendshipRequestSentEmail(friendshipRequestSentEmail);
    }
}
