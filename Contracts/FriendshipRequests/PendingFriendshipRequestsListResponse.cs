namespace Contracts.FriendshipRequests;

public sealed class PendingFriendshipRequestsListResponse
{
    public PendingFriendshipRequestsListResponse(IReadOnlyCollection<PendingFriendshipRequestModel> friendshipRequests)
        => FriendshipRequests = friendshipRequests;

    public IReadOnlyCollection<PendingFriendshipRequestModel> FriendshipRequests { get; }

    public sealed class PendingFriendshipRequestModel
    {
        public Guid Id { get; set; }

        public Guid FriendId { get; set; }

        public string FriendName { get; set; }

        public DateTime CreatedOnUtc { get; set; }
    }

}
