using Application.Core.Abstractions.Messaging;
using Contracts.Common;
using Contracts.GroupEvents;
using Domain.Core.Primitives.Maybe;

namespace Application.GroupEvents.Queries.GetGroupEvents;

/// <summary>
/// Represents the query for getting the paged list of the users group events.
/// </summary>
public sealed class GetGroupEventsQuery : IQuery<Maybe<PagedList<GroupEventResponse>>>
{
    public GetGroupEventsQuery(
        Guid userId,
        string name,
        int? categoryId,
        DateTime? startDate,
        DateTime? endDate,
        int page,
        int pageSize)
    {
        UserId = userId;
        Name = name;
        CategoryId = categoryId;
        StartDate = startDate;
        EndDate = endDate;
        Page = page;
        PageSize = Math.Min(Math.Max(pageSize, 0), PageSize);
    }

    public Guid UserId { get; }

    public string Name { get; }

    public int? CategoryId { get; }

    public DateTime? StartDate { get; }

    public DateTime? EndDate { get; }

    public int Page { get; }

    /// <summary>
    /// The max page size is 100.
    /// </summary>
    public int PageSize { get; }
}