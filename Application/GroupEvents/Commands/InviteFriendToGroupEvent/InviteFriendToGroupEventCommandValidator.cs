using Application.Core.Errors;
using Application.Core.Extensions;
using FluentValidation;

namespace Application.GroupEvents.Commands.InviteFriendToGroupEvent
{
    /// <summary>
    /// Represents the <see cref="InviteFriendToGroupEventCommand"/> validator.
    /// </summary>
    public sealed class InviteFriendToGroupEventCommandValidator : AbstractValidator<InviteFriendToGroupEventCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InviteFriendToGroupEventCommandValidator"/> class.
        /// </summary>
        public InviteFriendToGroupEventCommandValidator()
        {
            RuleFor(x => x.GroupEventId).NotEmpty().WithError(ValidationErrors.InviteFriendToGroupEvent.GroupEventIdIsRequired);

            RuleFor(x => x.FriendId).NotEmpty().WithError(ValidationErrors.InviteFriendToGroupEvent.FriendIdIsRequired);
        }
    }
}
