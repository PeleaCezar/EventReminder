using Domain.Core.Primitives.Maybe;
using Domain.Entities;

namespace Domain.Repositories
{
    /// <summary>
    /// Represents the group event repository interface.
    /// </summary>
    public interface IGroupEventRepository
    {
        Task<Maybe<GroupEvent>> GetByIdAsync(Guid groupEventId);

        /// <summary>
        /// Gets the distinct group events for the specified attendees.
        /// </summary>
        /// <param name="attendees">The attendees to get the group events for.</param>
        /// <returns>The readonly collection of group events with the specified identifiers.</returns>

        Task<IReadOnlyCollection<GroupEvent>> GetForAttendeesAsync(IReadOnlyCollection<Attendee> attendees);

        void Insert(GroupEvent groupEvent);
    }
}
