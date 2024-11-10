using Application.Core.Abstractions.Messaging;
using Domain.Core.Events;
using Domain.Events;

namespace Application.Invitations.Events.InvitationSent;

/// <summary>
/// Represents the <see cref="InvitationRejectedDomainEvent"/> handler.
/// </summary>
internal sealed class PublishIntegrationEventOnInvitationSentDomainEventHandler : IDomainEventHandler<InvitationSentDomainEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;

    /// <summary>
    /// Initializes a new instance of the <see cref="PublishIntegrationEventOnInvitationSentDomainEventHandler"/> class.
    /// </summary>
    /// <param name="integrationEventPublisher">The integration event publisher.</param>
    public PublishIntegrationEventOnInvitationSentDomainEventHandler(IIntegrationEventPublisher integrationEventPublisher) =>
        _integrationEventPublisher = integrationEventPublisher;

    /// <inheritdoc />
    public async Task Handle(InvitationSentDomainEvent notification, CancellationToken cancellationToken)
    {
        _integrationEventPublisher.Publish(new InvitationSentIntegrationEvent(notification));

        await Task.CompletedTask;
    }
}
