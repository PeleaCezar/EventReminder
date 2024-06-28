using Application.Core.Abstractions.Data;
using Domain.Core.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Specifications;

namespace Persistence.Repositories
{
    internal sealed class PersonalEventRepository : GenericRepository<PersonalEvent>, IPersonalEventRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersonalEventRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public PersonalEventRepository(IDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<IReadOnlyCollection<PersonalEvent>> GetUnprocessedAsync(int take) =>
            await DbContext.Set<PersonalEvent>()
               .Where(new UnProcessedPersonalEventSpecification())
               .OrderBy(personalEvent => personalEvent.CreatedOnUtc)
               .Take(take)
               .ToArrayAsync();
    }
}
