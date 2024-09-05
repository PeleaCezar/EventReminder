using Application.Core.Abstractions.Messaging;
using Contracts.GroupEvents;
using Domain.Core.Primitives.Maybe;

namespace Application.GroupEvents.Queries.Get10MostRecentAttendingGroupEventsQuery;

/// <summary>
/// Represents the query for getting the 10 most recent group event the user is attending.
/// </summary>
public sealed class Get10MostRecentAttendingGroupEventsQuery : IQuery<Maybe<IReadOnlyCollection<GroupEventResponse>>>
{
    public Get10MostRecentAttendingGroupEventsQuery(Guid userId) => UserId = userId;

    public Guid UserId { get; }

    public int NumberOfGroupEventsToTake => 10;
}
