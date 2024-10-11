using Application.Core.Errors;
using Application.Core.Extensions;
using FluentValidation;

namespace Application.Invitations.Commands.RejectInvitation
{
    public sealed class RejectInvitationCommandValidator : AbstractValidator<RejectInvitationCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RejectInvitationCommandValidator"/> class.
        /// </summary>
        public RejectInvitationCommandValidator() =>
            RuleFor(x => x.InvitationId).NotEmpty().WithError(ValidationErrors.RejectInvitation.InvitationIdIsRequired);
    }
}
