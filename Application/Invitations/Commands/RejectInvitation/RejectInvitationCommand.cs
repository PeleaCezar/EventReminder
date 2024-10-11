using Application.Core.Abstractions.Messaging;
using Domain.Core.Primitives.Result;

namespace Application.Invitations.Commands.RejectInvitation;

/// <summary>
/// Represents the reject invitation command.
/// </summary>
public sealed class RejectInvitationCommand : ICommand<Result>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RejectInvitationCommand"/> class.
    /// </summary>
    /// <param name="invitationId">The invitation identifier.</param>
    public RejectInvitationCommand(Guid invitationId) => InvitationId = invitationId;

    /// <summary>
    /// Gets the invitation identifier.
    /// </summary>
    public Guid InvitationId { get; }
}
