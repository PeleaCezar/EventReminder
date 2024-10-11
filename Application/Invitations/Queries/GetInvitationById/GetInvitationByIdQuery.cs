using Application.Core.Abstractions.Messaging;
using Contracts.Invitations;
using Domain.Core.Primitives.Maybe;

namespace Application.Invitations.Queries.GetInvitationById;

/// <summary>
/// Represents the query for getting the invitation by the identifier.
/// </summary>
public sealed class GetInvitationByIdQuery : IQuery<Maybe<InvitationResponse>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetInvitationByIdQuery"/> class.
    /// </summary>
    /// <param name="invitationId">The invitation identifier.</param>
    public GetInvitationByIdQuery(Guid invitationId) => InvitationId = invitationId;

    /// <summary>
    /// Gets the invitation identifier.
    /// </summary>
    public Guid InvitationId { get; }
}
