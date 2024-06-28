using Domain.Entities;
using System.Linq.Expressions;


namespace Persistence.Specifications;

internal sealed class UnprocessedAttendeeSpecification : Specification<Attendee>
{
    internal override Expression<Func<Attendee, bool>> ToExpression() => attendee => !attendee.Processed;
}
