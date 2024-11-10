﻿using Application.Core.Errors;
using Application.Core.Extensions;
using FluentValidation;

namespace Application.Authentication.Commands.Login;

/// <summary>
/// Represents the <see cref="LoginCommand"/> validator.
/// </summary>
public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoginCommandValidator"/> class.
    /// </summary>
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithError(ValidationErrors.Login.EmailIsRequired);

        RuleFor(x => x.Password).NotEmpty().WithError(ValidationErrors.Login.PasswordIsRequired);
    }
}
