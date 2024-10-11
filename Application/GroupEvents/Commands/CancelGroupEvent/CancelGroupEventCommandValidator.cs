using Application.Core.Errors;
using Application.Core.Extensions;
using FluentValidation;

namespace Application.GroupEvents.Commands.CancelGroupEvent
{
    /// <summary>
    /// Represents the <see cref="CancelGroupEventCommand"/> validator.
    /// </summary>
    public sealed class CancelGroupEventCommandValidator : AbstractValidator<CancelGroupEventCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancelGroupEventCommandValidator"/> class.
        /// </summary>
        public CancelGroupEventCommandValidator() =>
            RuleFor(x => x.GroupEventId).NotEmpty().WithError(ValidationErrors.CancelGroupEvent.GroupEventIdIsRequired);
    }
}
