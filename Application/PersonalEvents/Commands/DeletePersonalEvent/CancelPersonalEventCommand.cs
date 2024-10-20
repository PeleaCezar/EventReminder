using Application.Core.Abstractions.Messaging;
using Domain.Core.Primitives.Result;

namespace Application.PersonalEvents.Commands.DeletePersonalEvent;

/// <summary>
/// Represents the cancel personal event command.
/// </summary>
public sealed class CancelPersonalEventCommand : ICommand<Result>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CancelPersonalEventCommand"/> class.
    /// </summary>
    /// <param name="personalEventId">The personal event identifier.</param>
    public CancelPersonalEventCommand(Guid personalEventId) => PersonalEventId = personalEventId;

    /// <summary>
    /// Gets the personal event identifier.
    /// </summary>
    public Guid PersonalEventId { get; }
}
