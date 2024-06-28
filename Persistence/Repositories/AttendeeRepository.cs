using Application.Core.Abstractions.Data;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Persistence.Specifications;

namespace Persistence.Repositories;

internal sealed class AttendeeRepository : GenericRepository<Attendee>, IAttendeeRepository
{
    public AttendeeRepository(IDbContext dbContext)
    : base(dbContext)
    {
    }

    public async Task<IReadOnlyCollection<Attendee>> GetUnprocessedAsync(int take)
    {
        return await DbContext
             .Set<Attendee>()
             .Where(new UnprocessedAttendeeSpecification())
             .OrderBy(attendee => attendee.CreatedOnUtc)
             .Take(take)
             .ToArrayAsync();
    }

    public async Task<(string Email, string Name)[]> GetEmailsAndNamesForGroupEvent(GroupEvent groupEvent)
    {
        if (groupEvent.Id == Guid.Empty)
        {
            return Array.Empty<(string, string)>();
        }

        var attendeeEmailsAndNames = await(
            from @event in DbContext.Set<GroupEvent>()
            join attendee in DbContext.Set<Attendee>()
                on groupEvent.Id equals attendee.EventId
            join user in DbContext.Set<User>()
                on attendee.UserId equals user.Id
            where @event.Id == groupEvent.Id && attendee.UserId != groupEvent.UserId
            select new
            {
                Email = user.Email.Value,
                Name = user.FirstName.Value + " " + user.LastName.Value
            }).ToArrayAsync();

        return attendeeEmailsAndNames.Select(x => (x.Email, x.Name)).ToArray();
    }

    public async Task MarkUnprocessedForGroupEventAsync(GroupEvent groupEvent, DateTime utcNow)
    {
        const string sql = @"
                UPDATE Attendee
                SET Processed = 0, ModifiedOnUtc = @ModifiedOn
                WHERE EventId = @EventId AND Deleted = 0";

        SqlParameter[] parameters =
        {
                new SqlParameter("@ModifiedOn", utcNow),
                new SqlParameter("@EventId", groupEvent.Id)
        };

        await DbContext.ExecuteSqlAsync(sql, parameters);
    }

    public async Task RemoveAttendeesForGroupEventAsync(GroupEvent groupEvent, DateTime utcNow)
    {
        const string sql = @"
                UPDATE Attendee
                SET DeletedOnUtc = @DeletedOn, Deleted = @Deleted
                WHERE EventId = @EventId AND Deleted = 0";

        SqlParameter[] parameters =
        {
                new SqlParameter("@DeletedOn", utcNow),
                new SqlParameter("@Deleted", true),
                new SqlParameter("@EventId", groupEvent.Id)
        };

        await DbContext.ExecuteSqlAsync(sql, parameters);
    }
}
