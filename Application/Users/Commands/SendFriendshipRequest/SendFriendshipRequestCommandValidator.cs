﻿using Application.Core.Errors;
using Application.Core.Extensions;
using FluentValidation;

namespace Application.Users.Commands.SendFriendshipRequest
{
    /// <summary>
    /// Represents the <see cref="SendFriendshipRequestCommand"/> validator.
    /// </summary>
    public sealed class SendFriendshipRequestCommandValidator : AbstractValidator<SendFriendshipRequestCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendFriendshipRequestCommandValidator"/> class.
        /// </summary>
        public SendFriendshipRequestCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithError(ValidationErrors.SendFriendshipRequest.UserIdIsRequired);

            RuleFor(x => x.FriendId).NotEmpty().WithError(ValidationErrors.SendFriendshipRequest.FriendIdIsRequired);
        }
    }
}
