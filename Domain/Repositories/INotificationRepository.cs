using Domain.Entities;

namespace Domain.Repositories;

public interface INotificationRepository
{

    /// <summary>
    /// Gets the notifications along with their respective events and users that are waiting to be sent.
    /// </summary>
    /// <param name="batchSize">The batch size.</param>
    /// <param name="utcNow">The current date and time in UTC format.</param>
    /// <param name="allowedNotificationTimeDiscrepancyInMinutes">
    /// The allowed discrepancy between the current time and the time the notification is supposed to be sent.
    /// </param>
    /// <returns>The notifications along with their respective events and users that are waiting to be sent.</returns>
    Task<(Notification Notification, Event Event, User User)[]> GetNotificationsToBeSentIncludingUserAndEvent(
        int batchSize,
        DateTime utcNow,
        int allowedNotificationTimeDiscrepancyInMinutes);

    void InsertRange(IReadOnlyCollection<Notification> notifications);

    void Update(Notification notification);

    /// <summary>
    /// Removes all of the notifications for the specified event.
    /// </summary>
    /// <param name="event">The event.</param>
    /// <param name="utcNow">The current date and time in UTC format.</param>
    /// <returns>The completed task.</returns>
    Task RemoveNotificationsForEventAsync(Event @event, DateTime utcNow);
}
