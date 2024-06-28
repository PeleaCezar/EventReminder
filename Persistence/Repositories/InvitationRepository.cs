using Application.Core.Abstractions.Data;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Data.SqlClient;
using Persistence.Specifications;

namespace Persistence.Repositories;

internal sealed class InvitationRepository : GenericRepository<Invitation>, IInvitationRepository
{
    public InvitationRepository(IDbContext dbContext)
        : base(dbContext)
    {
    }
    public async Task<bool> CheckIfInvitationAlreadySentAsync(GroupEvent groupEvent, User user) =>
        await AnyAsync(new PendingInvitationSpecification(groupEvent, user));


    public async Task RemovePendingInvitationsForFriendshipAsync(Friendship friendship, DateTime utcNow)
    {
        const string sql = @"
                UPDATE Invitation
                SET DeletedOnUtc = @DeletedOn, Deleted = @Deleted
                WHERE (UserId = @UserId AND FriendId = @FriendId) ||
                      (UserId = @FriendId AND FriendId = @UserId)
                      CompletedOnUtc IS NULL AND Deleted = 0";

        SqlParameter[] parameters =
        {
                new SqlParameter("@DeletedOn", utcNow),
                new SqlParameter("@Deleted", true),
                new SqlParameter("@UserId", friendship.UserId),
                new SqlParameter("@FriendId", friendship.FriendId)
            };

        await DbContext.ExecuteSqlAsync(sql, parameters);
    }

    public async Task RemoveInvitationsForGroupEventAsync(GroupEvent groupEvent, DateTime utcNow)
    {
        const string sql = @"
                UPDATE Invitation
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
