using Application.Core.Abstractions.Messaging;
using Domain.Core.Events;
using Domain.Events;

namespace Application.GroupEvents.Events.GroupEventNameChanged;

/// <summary>
/// Represents the <see cref="GroupEventNameChangedDomainEvent"/> class.
/// </summary>
internal sealed class PublishIntegrationEventOnGroupEventNameChangedDomainEventHandler
    : IDomainEventHandler<GroupEventNameChangedDomainEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;

    /// <summary>
    /// Initializes a new instance of the <see cref="PublishIntegrationEventOnGroupEventNameChangedDomainEventHandler"/> class.
    /// </summary>
    /// <param name="integrationEventPublisher">The integration event publisher.</param>
    public PublishIntegrationEventOnGroupEventNameChangedDomainEventHandler(IIntegrationEventPublisher integrationEventPublisher) =>
        _integrationEventPublisher = integrationEventPublisher;

    /// <inheritdoc />
    public async Task Handle(GroupEventNameChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        _integrationEventPublisher.Publish(new GroupEventNameChangedIntegrationEvent(notification));

        await Task.CompletedTask;
    }
}
