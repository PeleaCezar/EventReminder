using Domain.Core.Events;
using Domain.Entities;

namespace Domain.Events
{
    /// <summary>
    /// Represents the event that is raised when a users first and last name is changed.
    /// </summary>
    public sealed class UserNameChangedDomainEvent : IDomainEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserNameChangedDomainEvent"/> class. 
        /// </summary>
        /// <param name="user">The user.</param>
        internal UserNameChangedDomainEvent(User user) => User = user;

        /// <summary>
        /// Gets the user.
        /// </summary>
        public User User { get; }
    }
}
