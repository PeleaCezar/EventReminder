namespace Contracts.Emails;

public sealed class FriendshipRequestSentEmail
{
    public FriendshipRequestSentEmail(string emailTo, string name, string userWhoSentRequest)
    {
        EmailTo = emailTo;
        Name = name;
        UserWhoSentRequest = userWhoSentRequest;
    }

    public string EmailTo { get; }

    public string Name { get; }

    public string UserWhoSentRequest { get; }
}
