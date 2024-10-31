using Application.Core.Errors;
using Application.Core.Extensions;
using FluentValidation;

namespace Application.Friendships.Commands;

public sealed class RemoveFriendshipCommandValidator : AbstractValidator<RemoveFriendshipCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveFriendshipCommandValidator"/> class.
    /// </summary>
    public RemoveFriendshipCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithError(ValidationErrors.RemoveFriendship.UserIdIsRequired);

        RuleFor(x => x.FriendId).NotEmpty().WithError(ValidationErrors.RemoveFriendship.FriendIdIsRequired);
    }
}
