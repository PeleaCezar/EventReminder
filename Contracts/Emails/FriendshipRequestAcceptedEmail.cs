namespace Contracts.Emails;

public sealed class FriendshipRequestAcceptedEmail
{
    public FriendshipRequestAcceptedEmail(string emailTo, string name, string friendName)
    {
        EmailTo = emailTo;
        Name = name;
        FriendName = friendName;
    }

    public string EmailTo { get; }

    public string Name { get; }

    public string FriendName { get; }
}
