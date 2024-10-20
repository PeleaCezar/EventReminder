using Application.Core.Errors;
using Application.Core.Extensions;
using FluentValidation;

namespace Application.PersonalEvents.Commands.DeletePersonalEvent;

/// <summary>
/// Represents the <see cref="CancelPersonalEventCommand"/> validator.
/// </summary>
public sealed class CancelGroupEventCommandValidator : AbstractValidator<CancelPersonalEventCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CancelGroupEventCommandValidator"/> class.
    /// </summary>
    public CancelGroupEventCommandValidator() =>
        RuleFor(x => x.PersonalEventId).NotEmpty().WithError(ValidationErrors.CancelPersonalEvent.PersonalEventIdIsRequired);
}
