using Application.Core.Abstractions.Data;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

internal sealed class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{

    public NotificationRepository(IDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<(Notification Notification, Event Event, User User)[]> GetNotificationsToBeSentIncludingUserAndEvent(
        int batchSize,
        DateTime utcNow,
        int allowedNotificationTimeDiscrepancyInMinutes)
    {
        DateTime startTime = utcNow.AddMinutes(-allowedNotificationTimeDiscrepancyInMinutes);
        DateTime endTime = utcNow.AddMinutes(allowedNotificationTimeDiscrepancyInMinutes);

        var notificationsWithUsersAndEvents = await (
                from notification in DbContext.Set<Notification>()
                join @event in DbContext.Set<Event>()
                    on notification.EventId equals @event.Id
                join user in DbContext.Set<User>()
                    on notification.UserId equals user.Id
                where !notification.Sent &&
                      notification.DateTimeUtc >= startTime &&
                      notification.DateTimeUtc <= endTime
                orderby notification.DateTimeUtc
                select new
                {
                    Notification = notification,
                    Event = @event,
                    User = user
                })
               .Take(batchSize)
               .ToArrayAsync();

        return notificationsWithUsersAndEvents.Select(x => (x.Notification, x.Event, x.User)).ToArray();
    }

    public async Task RemoveNotificationsForEventAsync(Event @event, DateTime utcNow)
    {
        const string sql = @"
                UPDATE Notification
                SET DeletedOnUtc = @DeletedOn, Deleted = @Deleted
                WHERE EventId = @EventId AND Deleted = 0";

        SqlParameter[] parameters =
        {
                new SqlParameter("@DeletedOn", utcNow),
                new SqlParameter("@Deleted", true),
                new SqlParameter("@EventId", @event.Id)
        };

        await DbContext.ExecuteSqlAsync(sql, parameters);
    }
}
