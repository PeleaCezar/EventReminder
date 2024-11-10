using Application.Core.Abstractions.Messaging;
using Domain.Core.Events;
using Domain.Events;

namespace Application.Invitations.Events.InvitationRejected;

/// <summary>
/// Represents the <see cref="InvitationSentDomainEvent"/> handler.
/// </summary>
internal sealed class PublishIntegrationEventOnInvitationRejectedDomainEventHandler
    : IDomainEventHandler<InvitationRejectedDomainEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;

    /// <summary>
    /// Initializes a new instance of the <see cref="PublishIntegrationEventOnInvitationRejectedDomainEventHandler"/> class.
    /// </summary>
    /// <param name="integrationEventPublisher">The integration event publisher.</param>
    public PublishIntegrationEventOnInvitationRejectedDomainEventHandler(IIntegrationEventPublisher integrationEventPublisher) =>
        _integrationEventPublisher = integrationEventPublisher;

    public async Task Handle(InvitationRejectedDomainEvent notification, CancellationToken cancellationToken)
    {
        _integrationEventPublisher.Publish(new InvitationRejectedIntegrationEvent(notification));

        await Task.CompletedTask;
    }
}
