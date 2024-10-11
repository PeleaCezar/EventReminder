﻿using Application.Core.Errors;
using Application.Core.Extensions;
using FluentValidation;

namespace Application.GroupEvents.Commands.UpdateGroupEvent;

/// <summary>
/// Represents the <see cref="UpdateGroupEventCommand"/> validator.
/// </summary>
public sealed class UpdateGroupEventCommandValidator : AbstractValidator<UpdateGroupEventCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref=""/>
    /// </summary>
    public UpdateGroupEventCommandValidator()
    {
        RuleFor(x => x.GroupEventId).NotEmpty().WithError(ValidationErrors.UpdateGroupEvent.GroupEventIdIsRequired);

        RuleFor(x => x.Name).NotEmpty().WithError(ValidationErrors.UpdateGroupEvent.NameIsRequired);

        RuleFor(x => x.DateTimeUtc).NotEmpty().WithError(ValidationErrors.UpdateGroupEvent.DateAndTimeIsRequired);
    }
}
