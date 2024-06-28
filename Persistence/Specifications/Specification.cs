using Domain.Core.Primitives;
using System.Linq.Expressions;

namespace Persistence.Specifications;

/// <summary>
/// Represents the abstract base class for specifications.
/// </summary>
internal abstract class Specification<TEntity> where TEntity : Entity
{
    /// <summary>
    /// Converts the specification to an expression predicate.
    /// </summary>
    internal abstract Expression<Func<TEntity, bool>> ToExpression();

    internal bool IsSatisfiedBy(TEntity entity) => ToExpression().Compile()(entity);

    public static implicit operator Expression<Func<TEntity, bool>>(Specification<TEntity> specification) =>
        specification.ToExpression();
}
