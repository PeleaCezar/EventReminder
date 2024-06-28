namespace Contracts.Emails;

public sealed class AttendeeCreatedEmail
{
    public AttendeeCreatedEmail(string emailTo, string name, string eventName, string eventDateAndTime)
    {
        EmailTo = emailTo;
        Name = name;
        EventName = eventName;
        EventDateAndTime = eventDateAndTime;
    }

    public string EmailTo { get; }

    public string Name { get; }

    public string EventName { get; }

    public string EventDateAndTime { get; }

}
