using Application.Core.Abstractions.Messaging;
using Contracts.Attendees;
using Domain.Core.Primitives.Maybe;

namespace Application.Attendees.Queries.GetAttendeesForEventId;

/// <summary>
/// Represents the query for getting group event attendees.
/// </summary>
public sealed class GetAttendeesForGroupEventIdQuery : IQuery<Maybe<AttendeeListResponse>>
{
    public GetAttendeesForGroupEventIdQuery(Guid groupEventId) => GroupEventId = groupEventId;

    public Guid GroupEventId { get; }
}
