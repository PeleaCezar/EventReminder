using Domain.Core.Entities;
using System.Linq.Expressions;

namespace Persistence.Specifications;

/// <summary>
/// Represents the specification for determining the unprocessed personal event.
/// </summary>
internal sealed class UnProcessedPersonalEventSpecification : Specification<PersonalEvent>
{
    /// <inheritdoc />
    internal override Expression<Func<PersonalEvent, bool>> ToExpression() => personalEvent => !personalEvent.Processed;
}
