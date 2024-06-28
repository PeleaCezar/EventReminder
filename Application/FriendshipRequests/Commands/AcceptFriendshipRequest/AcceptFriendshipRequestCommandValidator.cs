using Application.Core.Errors;
using Application.Core.Extensions;
using FluentValidation;

namespace Application.FriendshipRequests.Commands;

public sealed class AcceptFriendshipRequestCommandValidator : AbstractValidator<AcceptFriendshipRequestCommand>
{
    public AcceptFriendshipRequestCommandValidator() =>
        RuleFor(x => x.FriendshipRequestId)
            .NotEmpty()
            .WithError(ValidationErrors.AcceptFriendshipRequest.FriendshipRequestIdIsRequired);
}
