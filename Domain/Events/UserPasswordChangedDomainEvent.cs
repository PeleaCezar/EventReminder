﻿using Domain.Core.Events;
using Domain.Entities;

namespace Domain.Events;

/// <summary>
/// Represents the event that is raised when a users password is changed.
/// </summary>
public sealed class UserPasswordChangedDomainEvent : IDomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserPasswordChangedDomainEvent"/> class. 
    /// </summary>
    /// <param name="user">The user.</param>
    internal UserPasswordChangedDomainEvent(User user) => User = user;

    /// <summary>
    /// Gets the user.
    /// </summary>
    public User User { get; }
}
