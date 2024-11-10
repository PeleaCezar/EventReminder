using Application.Core.Errors;
using Application.Core.Extensions;
using FluentValidation;

namespace Application.FriendshipRequests.Commands;

/// <summary>
/// Represents the <see cref="AcceptFriendshipRequestCommand"/> validator.
/// </summary>
public sealed class AcceptFriendshipRequestCommandValidator : AbstractValidator<AcceptFriendshipRequestCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AcceptFriendshipRequestCommandValidator"/> class.
    /// </summary>
    public AcceptFriendshipRequestCommandValidator() =>
        RuleFor(x => x.FriendshipRequestId)
            .NotEmpty()
            .WithError(ValidationErrors.AcceptFriendshipRequest.FriendshipRequestIdIsRequired);
}