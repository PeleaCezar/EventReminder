using Application.Core.Errors;
using Application.Core.Extensions;
using FluentValidation;

namespace Application.FriendshipRequests.Commands.RejectFriendshipRequest
{
    public sealed class RejectFriendshipRequestCommandValidator : AbstractValidator<RejectFriendshipRequestCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RejectFriendshipRequestCommandValidator"/> class.
        /// </summary>
        public RejectFriendshipRequestCommandValidator() =>
            RuleFor(x => x.FriendshipRequestId)
                .NotEmpty()
                .WithError(ValidationErrors.RejectFriendshipRequest.FriendshipRequestIdIsRequired);
    }
}
