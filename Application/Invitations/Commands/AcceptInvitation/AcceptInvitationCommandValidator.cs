using Application.Core.Errors;
using Application.Core.Extensions;
using FluentValidation;

namespace Application.Invitations.Commands.AcceptInvitation;

public sealed class AcceptInvitationCommandValidator : AbstractValidator<AcceptInvitationCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AcceptInvitationCommandValidator"/> class.
    /// </summary>
    public AcceptInvitationCommandValidator() =>
        RuleFor(x => x.InvitationId).NotEmpty().WithError(ValidationErrors.AcceptInvitation.InvitationIdIsRequired);
}
