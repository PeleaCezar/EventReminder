namespace Contracts.Emails;

public sealed class GroupEventDateAndTimeChangedEmail
{
    public GroupEventDateAndTimeChangedEmail(string emailTo, string name, string eventName, string oldEventDateAndTime, string eventDateAndTime)
    {
        EmailTo = emailTo;
        Name = name;
        EventName = eventName;
        OldEventDateAndTime = oldEventDateAndTime;
        EventDateAndTime = eventDateAndTime;
    }

    public string EmailTo { get; }

    public string Name { get; }

    public string EventName { get; }

    public string OldEventDateAndTime { get; }

    public string EventDateAndTime { get; }
}