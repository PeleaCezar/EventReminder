namespace Contracts.Attendees;

public sealed class AttendeeListResponse
{
    public AttendeeListResponse(AttendeeModel[] attendees) => Attendees = attendees;

    public AttendeeModel[] Attendees { get; }

    public sealed class AttendeeModel
    {
        public Guid UserId { get; set; }

        public string Name { get; set; }

        public DateTime CreatedOnUtc { get; set; }
    }
}
