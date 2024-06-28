using Domain.Core.Events;
using Domain.Entities;

namespace Domain.Events
{
    /// <summary>
    /// Represents the event that is raised when a group event is cancelled.
    /// </summary>
    public sealed class GroupEventCancelledDomainEvent : IDomainEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupEventCancelledDomainEvent"/> class.
        /// </summary>
        /// <param name="groupEvent">The group event.</param>
        internal GroupEventCancelledDomainEvent(GroupEvent groupEvent) => GroupEvent = groupEvent;

        /// <summary>
        /// Gets the group event.
        /// </summary>
        public GroupEvent GroupEvent { get; }
    }
}
