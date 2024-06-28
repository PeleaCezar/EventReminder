using Application.Core.Abstractions.Data;
using Domain.Entities;
using Domain.Repositories;
using Persistence.Specifications;

namespace Persistence.Repositories;

internal sealed class FriendshipRequestRepository : GenericRepository<FriendshipRequest>, IFriendshipRequestRepository
{
    public FriendshipRequestRepository(IDbContext dbContext) 
        : base(dbContext)
    {
    }

    public async Task<bool> CheckForPendingFriendshipRequestAsync(User user, User friend)
    {
        return await AnyAsync(new PendingFriendshipRequestSpecification(user, friend));
    }
}
