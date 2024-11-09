using Application.Core.Abstractions.Messaging;

namespace BackgroundTasks.Services;

/// <summary>
/// Represents the integration event consumer interface.
/// </summary>
internal interface IIntegrationEventConsumer
{
    /// <summary>
    /// Consumes the incoming the specified integration event.
    /// </summary>
    void Consume(IIntegrationEvent integrationEvent);
}
