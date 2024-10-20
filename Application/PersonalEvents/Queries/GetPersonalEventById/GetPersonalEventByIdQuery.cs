using Application.Core.Abstractions.Messaging;
using Contracts.PersonalEvents;
using Domain.Core.Primitives.Maybe;

namespace Application.PersonalEvents.Queries.GetPersonalEventById
{
    /// <summary>
    /// Represents the query for getting the personal event by identifier.
    /// </summary>
    public sealed class GetPersonalEventByIdQuery : IQuery<Maybe<DetailedPersonalEventResponse>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetPersonalEventByIdQuery"/> class.
        /// </summary>
        /// <param name="personalEventId">The personal event identifier.</param>
        public GetPersonalEventByIdQuery(Guid personalEventId) => PersonalEventId = personalEventId;

        /// <summary>
        /// Gets the personal event identifier.
        /// </summary>
        public Guid PersonalEventId { get; }
    }
}
