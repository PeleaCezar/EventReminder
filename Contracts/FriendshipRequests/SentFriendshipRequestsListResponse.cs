namespace Contracts.FriendshipRequests;

public sealed class SentFriendshipRequestsListResponse
{
    public SentFriendshipRequestsListResponse(IReadOnlyCollection<SentFriendshipRequestModel> friendshipRequests)
           => FriendshipRequests = friendshipRequests;

    public IReadOnlyCollection<SentFriendshipRequestModel> FriendshipRequests { get; }

    public sealed class SentFriendshipRequestModel
    {
        public Guid Id { get; set; }

        public Guid FriendId { get; set; }

        public string FriendName { get; set; }

        public DateTime CreatedOnUtc { get; set; }
    }
}
