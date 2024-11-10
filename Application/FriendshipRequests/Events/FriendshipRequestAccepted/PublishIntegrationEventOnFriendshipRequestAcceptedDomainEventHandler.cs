using Application.Core.Abstractions.Messaging;
using Domain.Core.Events;
using Domain.Events;

namespace Application.FriendshipRequests.Events.FriendshipRequestAccepted;

/// <summary>
/// Represents the <see cref="FriendshipRequestSentDomainEvent"/> handler.
/// </summary>
internal sealed class PublishIntegrationEventOnFriendshipRequestAcceptedDomainEventHandler
    : IDomainEventHandler<FriendshipRequestAcceptedDomainEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;

    /// <summary>
    /// Initializes a new instance of the <see cref="PublishIntegrationEventOnFriendshipRequestAcceptedDomainEventHandler"/> class.
    /// </summary>
    /// <param name="integrationEventPublisher">The integration event publisher.</param>
    public PublishIntegrationEventOnFriendshipRequestAcceptedDomainEventHandler(IIntegrationEventPublisher integrationEventPublisher)
        => _integrationEventPublisher = integrationEventPublisher;

    /// <inheritdoc />
    public async Task Handle(FriendshipRequestAcceptedDomainEvent notification, CancellationToken cancellationToken)
    {
        _integrationEventPublisher.Publish(new FriendshipRequestAcceptedIntegrationEvent(notification));

        await Task.CompletedTask;
    }
}
