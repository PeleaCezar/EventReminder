﻿using Application.Core.Abstractions.Data;
using Domain.Core.Events;
using Domain.Events;
using Domain.Repositories;
using Domain.Services;

namespace Application.FriendshipRequests.Events.FriendshipRequestAccepted;

/// <summary>
/// Represents the <see cref="FriendshipRequestAcceptedDomainEvent"/> handler.
/// </summary>
internal sealed class CreateFriendshipOnFriendshipRequestAcceptedDomainEventHandler
    : IDomainEventHandler<FriendshipRequestAcceptedDomainEvent>
{
    private readonly IUserRepository _userRepository;
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateFriendshipOnFriendshipRequestAcceptedDomainEventHandler"/> class.
    /// </summary>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="friendshipRepository">The friendship repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public CreateFriendshipOnFriendshipRequestAcceptedDomainEventHandler(
        IUserRepository userRepository,
        IFriendshipRepository friendshipRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _friendshipRepository = friendshipRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task Handle(FriendshipRequestAcceptedDomainEvent notification, CancellationToken cancellationToken)
    {
        var friendshipService = new FriendshipService(_userRepository, _friendshipRepository);

        await friendshipService.CreateFriendshipAsync(notification.FriendshipRequest);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
