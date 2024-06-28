using Application.Core.Errors;
using Application.Core.Extensions;
using FluentValidation;

namespace Application.Authentication.Commands.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithError(ValidationErrors.Login.EmailIsRequired);

        RuleFor(x => x.Password).NotEmpty().WithError(ValidationErrors.Login.PasswordIsRequired);
    }
}
