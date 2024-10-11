﻿using Application.Core.Abstractions.Messaging;
using Domain.Core.Primitives.Result;

namespace Application.GroupEvents.Commands.InviteFriendToGroupEvent;

/// <summary>
/// Represents the invite friend to group event command.
/// </summary>
public sealed class InviteFriendToGroupEventCommand : ICommand<Result>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InviteFriendToGroupEventCommand"/> class.
    /// </summary>
    /// <param name="groupEventId">The group event identifier.</param>
    /// <param name="friendId">The friend identifier.</param>
    public InviteFriendToGroupEventCommand(Guid groupEventId, Guid friendId)
    {
        GroupEventId = groupEventId;
        FriendId = friendId;
    }

    /// <summary>
    /// Gets the group event identifier.
    /// </summary>
    public Guid GroupEventId { get; }

    /// <summary>
    /// Gets the friend identifier.
    /// </summary>
    public Guid FriendId { get; }
}
