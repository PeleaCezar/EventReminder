using Domain.Core.Primitives.Maybe;
using Domain.Entities;

namespace Domain.Repositories;

public interface IAttendeeRepository
{
    Task<Maybe<Attendee>> GetByIdAsync(Guid attendeeId);

    /// <summary>
    /// Gets the specified number of unprocessed attendees, if they exist.
    /// </summary>
    Task<IReadOnlyCollection<Attendee>> GetUnprocessedAsync(int take);

    /// <summary>
    /// Gets the emails and names of all of the attendees.
    /// </summary>
    Task<(string Email, string Name)[]> GetEmailsAndNamesForGroupEvent(GroupEvent groupEvent);

    void Insert(Attendee attendee);

    Task MarkUnprocessedForGroupEventAsync(GroupEvent groupEvent, DateTime utcNow);

    Task RemoveAttendeesForGroupEventAsync(GroupEvent groupEvent, DateTime utcNow);
}
