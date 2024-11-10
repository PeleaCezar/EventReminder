using Application.Core.Abstractions.Messaging;
using Domain.Core.Events;
using Domain.Events;

namespace Application.GroupEvents.Events.GroupEventDateAndTimeChanged;

/// <summary>
/// Represents the <see cref="GroupEventDateAndTimeChangedDomainEvent"/> class.
/// </summary>
internal sealed class PublishIntegrationEventOnGroupEventDateAndTimeChangedDomainEventHandler
    : IDomainEventHandler<GroupEventDateAndTimeChangedDomainEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;

    /// <summary>
    /// Initializes a new instance of the <see cref="PublishIntegrationEventOnGroupEventDateAndTimeChangedDomainEventHandler"/> class.
    /// </summary>
    /// <param name="integrationEventPublisher">The integration event publisher.</param>
    public PublishIntegrationEventOnGroupEventDateAndTimeChangedDomainEventHandler(
        IIntegrationEventPublisher integrationEventPublisher) =>
        _integrationEventPublisher = integrationEventPublisher;

    /// <inheritdoc />
    public async Task Handle(GroupEventDateAndTimeChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        _integrationEventPublisher.Publish(new GroupEventDateAndTimeChangedIntegrationEvent(notification));

        await Task.CompletedTask;
    }
}