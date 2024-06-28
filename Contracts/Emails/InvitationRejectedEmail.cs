namespace Contracts.Emails;

public sealed class InvitationRejectedEmail
{
    public InvitationRejectedEmail(string emailTo, string name, string friendName, string eventName, string eventDateAndTime)
    {
        EmailTo = emailTo;
        Name = name;
        FriendName = friendName;
        EventName = eventName;
        EventDateAndTime = eventDateAndTime;
    }

    public string EmailTo { get; }

    public string Name { get; }

    public string FriendName { get; }

    public string EventName { get; }

    public string EventDateAndTime { get; }
}
