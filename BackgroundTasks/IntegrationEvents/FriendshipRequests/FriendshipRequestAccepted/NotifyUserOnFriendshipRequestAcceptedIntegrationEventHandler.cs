using Application.Core.Abstractions.Notifications;
using Application.FriendshipRequests.Events.FriendshipRequestAccepted;
using BackgroundTasks.Abstractions.Messaging;
using Contracts.Emails;
using Domain.Core.Errors;
using Domain.Core.Exceptions;
using Domain.Core.Primitives.Maybe;
using Domain.Entities;
using Domain.Repositories;

namespace BackgroundTasks.IntegrationEvents.FriendshipRequests.FriendshipRequestAccepted
{
    internal sealed class NotifyUserOnFriendshipRequestAcceptedIntegrationEventHandler
        : IIntegrationEventHandler<FriendshipRequestAcceptedIntegrationEvent>
    {

        private readonly IFriendshipRequestRepository _friendshipRequestRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailNotificationService _emailNotificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyUserOnFriendshipRequestAcceptedIntegrationEventHandler"/> class.
        /// </summary>
        /// <param name="friendshipRequestRepository">The friendship request repository.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="emailNotificationService">The email notification service.</param>
        public NotifyUserOnFriendshipRequestAcceptedIntegrationEventHandler(
            IFriendshipRequestRepository friendshipRequestRepository,
            IUserRepository userRepository,
            IEmailNotificationService emailNotificationService)
        {
            _friendshipRequestRepository = friendshipRequestRepository;
            _userRepository = userRepository;
            _emailNotificationService = emailNotificationService;
        }

        public async Task Handle(FriendshipRequestAcceptedIntegrationEvent notification, CancellationToken cancellationToken)
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

            var friendshipRequestAcceptedEmail = new FriendshipRequestAcceptedEmail(user.Email, user.FullName, friend.FullName);

            await _emailNotificationService.SendFriendshipRequestAcceptedEmail(friendshipRequestAcceptedEmail);
        }
    }
}
