using Application.Core.Abstractions.Messaging;
using Domain.Core.Events;
using Domain.Events;

namespace Application.Invitations.Events.InvitationAccepted;

/// <summary>
/// Represents the <see cref="InvitationAcceptedDomainEvent"/> handler.
/// </summary>
internal sealed class PublishIntegrationEventOnInvitationAcceptedDomainEventHandler
    : IDomainEventHandler<InvitationAcceptedDomainEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;

    /// <summary>
    /// Initializes a new instance of the <see cref="PublishIntegrationEventOnInvitationAcceptedDomainEventHandler"/> class.
    /// </summary>
    /// <param name="integrationEventPublisher">The integration event publisher.</param>
    public PublishIntegrationEventOnInvitationAcceptedDomainEventHandler(IIntegrationEventPublisher integrationEventPublisher) =>
        _integrationEventPublisher = integrationEventPublisher;

    /// <inheritdoc />
    public async Task Handle(InvitationAcceptedDomainEvent notification, CancellationToken cancellationToken)
    {
        _integrationEventPublisher.Publish(new InvitationAcceptedIntegrationEvent(notification));

        await Task.CompletedTask;
    }
}
