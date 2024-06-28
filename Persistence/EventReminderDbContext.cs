using Application.Core.Abstractions.Common;
using Application.Core.Abstractions.Data;
using Domain.Core.Abstractions;
using Domain.Core.Events;
using Domain.Core.Primitives;
using Domain.Core.Primitives.Maybe;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Persistence.Extensions;
using System;
using System.Data.SqlClient;
using System.Reflection;

namespace Persistence;

public sealed class EventReminderDbContext : DbContext, IDbContext, IUnitOfWork
{
    private readonly IDateTime _dateTime;
    private readonly IMediator _mediator;

    public EventReminderDbContext(DbContextOptions options, IDateTime dateTime, IMediator mediator)
        : base(options)
    {
        _dateTime = dateTime;
        _mediator = mediator;
    }

    public new DbSet<TEntity> Set<TEntity>() where TEntity : Entity
    {
        return base.Set<TEntity>();
    }

    public async Task<Maybe<TEntity>> GetBydIdAsync<TEntity>(Guid id) where TEntity : Entity
    {
        if (id == Guid.Empty)
        {     
            return Maybe<TEntity>.None;
        }

        return Maybe<TEntity>.From(await Set<TEntity>().FirstOrDefaultAsync(e => e.Id == id));
    }

    public void Insert<TEntity>(TEntity entity) where TEntity : Entity
    {
        Set<TEntity>().Add(entity);
    }

    public void InsertRange<TEntity>(IReadOnlyCollection<TEntity> entities) where TEntity : Entity
    {
        Set<TEntity>().AddRange(entities);
    }

    public new void Remove<TEntity>(TEntity entity) where TEntity : Entity
    {
        Set<TEntity>().Remove(entity);
    }

    public Task<int> ExecuteSqlAsync(string sql, IEnumerable<SqlParameter> parameters, CancellationToken cancellationToken = default)
    {
         return Database.ExecuteSqlRawAsync(sql, parameters, cancellationToken);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        DateTime utcNow = _dateTime.UtcNow;

        UpdateAuditableEntities(utcNow);

        UpdateSoftDeletableEntities(utcNow);

        await PublishDomainEvents(cancellationToken);

        return await base.SaveChangesAsync(cancellationToken);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Database.BeginTransactionAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.ApplyUtcDateTimeConverter();

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Updates the entities implementing <see cref="IAuditableEntity"/> interface.
    /// </summary>
    /// <param name="utcNow">The current date and time in UTC format.</param>
    private void UpdateAuditableEntities(DateTime utcNow)
    {
        foreach (EntityEntry<IAuditableEntity> entityEntry in ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property(nameof(IAuditableEntity.CreatedOnUtc)).CurrentValue = utcNow;
            }

            if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Property(nameof(IAuditableEntity.ModifiedOnUtc)).CurrentValue = utcNow;
            }
        }
    }

    /// <summary>
    /// Updates the entities implementing <see cref="ISoftDeletableEntity"/> interface.
    /// </summary>
    /// <param name="utcNow">The current date and time in UTC format.</param>
    private void UpdateSoftDeletableEntities(DateTime utcNow)
    {
        foreach (EntityEntry<ISoftDeletableEntity> entityEntry in ChangeTracker.Entries<ISoftDeletableEntity>())
        {
            if (entityEntry.State != EntityState.Deleted)
            {
                continue;
            }

            entityEntry.Property(nameof(ISoftDeletableEntity.DeletedOnUtc)).CurrentValue = utcNow;

            entityEntry.Property(nameof(ISoftDeletableEntity.Deleted)).CurrentValue = true;

            entityEntry.State = EntityState.Modified;

            UpdateDeletedEntityEntryReferencesToUnchanged(entityEntry);
        }
    }

    /// <summary>
    /// Updates the specified entity entry's referenced entries in the deleted state to the modified state.
    /// This method is recursive.
    /// </summary>
    /// <param name="entityEntry">The entity entry.</param>
    private static void UpdateDeletedEntityEntryReferencesToUnchanged(EntityEntry entityEntry)
    {
        if (!entityEntry.References.Any())
        {
            return;
        }

        foreach (ReferenceEntry referenceEntry in entityEntry.References.Where(r => r.TargetEntry.State == EntityState.Deleted))
        {
            referenceEntry.TargetEntry.State = EntityState.Unchanged;

            UpdateDeletedEntityEntryReferencesToUnchanged(referenceEntry.TargetEntry);
        }
    }

    /// <summary>
    /// Publishes and then clears all the domain events that exist within the current transaction.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    private async Task PublishDomainEvents(CancellationToken cancellationToken)
    {
        List<EntityEntry<AggregateRoot>> aggregateRoots = ChangeTracker
            .Entries<AggregateRoot>()
            .Where(entityEntry => entityEntry.Entity.DomainEvents.Any())
            .ToList();

        List<IDomainEvent> domainEvents = aggregateRoots.SelectMany(entityEntry => entityEntry.Entity.DomainEvents).ToList();

        aggregateRoots.ForEach(entityEntry => entityEntry.Entity.ClearDomainEvents());

        IEnumerable<Task> tasks = domainEvents.Select(domainEvent => _mediator.Publish(domainEvent, cancellationToken));

        await Task.WhenAll(tasks);
    }

}
