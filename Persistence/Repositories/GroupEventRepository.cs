using Application.Core.Abstractions.Data;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Specifications;

namespace Persistence.Repositories;

/// <summary>
/// Represents the group event repository.
/// </summary>
internal sealed class GroupEventRepository : GenericRepository<GroupEvent>, IGroupEventRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupEventRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public GroupEventRepository(IDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<IReadOnlyCollection<GroupEvent>> GetForAttendeesAsync(IReadOnlyCollection<Attendee> attendees)
    {
       return attendees.Any() 
            ? await DbContext.Set<GroupEvent>().Where(new GroupEventForAttendeesSpecification(attendees)).ToArrayAsync()
            : Array.Empty<GroupEvent>();
    }
}