using Domain.Entities;
using Domain.ValueObjects;
using System.Linq.Expressions;

namespace Persistence.Specifications;

internal sealed class UserWithEmailSpecification : Specification<User>
{
    private readonly Email _email;

    internal UserWithEmailSpecification(Email email) => _email = email;

    /// <inheritdoc />
    internal override Expression<Func<User, bool>> ToExpression() => user => user.Email.Value == _email;
}
