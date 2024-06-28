using Domain.Core.Primitives;
using Domain.Core.Primitives.Maybe;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace Application.Core.Abstractions.Data;

public interface IDbContext
{
    DbSet<TEntity> Set<TEntity>()
               where TEntity : Entity;

    Task<Maybe<TEntity>> GetBydIdAsync<TEntity>(Guid id)
             where TEntity : Entity;

    void Insert<TEntity>(TEntity entity)
             where TEntity : Entity;

    void InsertRange<TEntity>(IReadOnlyCollection<TEntity> entities)
        where TEntity : Entity;

    /// <summary>
    /// Executes the specified SQL command asynchronously and returns the number of affected rows.
    /// </summary>
    void Remove<TEntity>(TEntity entity)
        where TEntity : Entity;

    /// <summary>
    /// Executes the specified SQL command asynchronously and returns the number of affected rows.
    /// </summary>
    Task<int> ExecuteSqlAsync(string sql, IEnumerable<SqlParameter> parameters, CancellationToken cancellationToken = default);
}
