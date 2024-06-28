using Application.Core.Abstractions.Messaging;

namespace BackgroundTasks.Services
{
    internal interface IIntegrationEventConsumer
    {
        /// <summary>
        /// Consumes the incoming the specified integration event.
        /// </summary>
        void Consume(IIntegrationEvent integrationEvent);
    }
}
