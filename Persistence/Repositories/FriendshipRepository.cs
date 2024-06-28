using Application.Core.Abstractions.Data;
using Domain.Entities;
using Domain.Repositories;
using Persistence.Specifications;

namespace Persistence.Repositories;

internal sealed class FriendshipRepository : GenericRepository<Friendship>, IFriendshipRepository
{
    public FriendshipRepository(IDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<bool> CheckIfFriendsAsync(User user, User friend) => await AnyAsync(new FriendshipSpecification(user, friend));
}
