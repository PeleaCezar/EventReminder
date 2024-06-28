namespace Contracts.FriendshipRequests;


public sealed class FriendshipRequestResponse
{
    public Guid UserId { get; set; }

    public string UserEmail { get; set; }

    public string UserName { get; set; }

    public Guid FriendId { get; set; }

    public string FriendEmail { get; set; }

    public string FriendName { get; set; }

    public DateTime CreatedOnUtc { get; set; }
}
