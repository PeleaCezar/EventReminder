namespace Contracts.Emails;

public sealed class GroupEventNameChangedEmail
{
    public GroupEventNameChangedEmail(string emailTo, string name, string eventName, string oldEventName, string eventDateAndTime)
    {
        EmailTo = emailTo;
        Name = name;
        EventName = eventName;
        OldEventName = oldEventName;
        EventDateAndTime = eventDateAndTime;
    }

    public string EmailTo { get; }

    public string Name { get; }

    public string EventName { get; }

    public string OldEventName { get; }

    public string EventDateAndTime { get; }
}
