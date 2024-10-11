using Application.Core.Abstractions.Authentication;
using Application.Core.Abstractions.Common;
using Application.Core.Abstractions.Data;
using Application.Core.Abstractions.Messaging;
using Domain.Core.Errors;
using Domain.Core.Primitives.Maybe;
using Domain.Core.Primitives.Result;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Invitations.Commands.AcceptInvitation;

internal sealed class AcceptInvitationCommandHandler : ICommandHandler<AcceptInvitationCommand, Result>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IInvitationRepository _invitationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTime _dateTime;
    private readonly IMediator _mediator;

    public AcceptInvitationCommandHandler(
        IUserIdentifierProvider userIdentifierProvider,
        IInvitationRepository invitationRepository,
        IUnitOfWork unitOfWork,
        IDateTime dateTime,
        IMediator mediator)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _invitationRepository = invitationRepository;
        _unitOfWork = unitOfWork;
        _dateTime = dateTime;
        _mediator = mediator;
    }

    public async Task<Result> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
    {
        Maybe<Invitation> maybeInvitation = await _invitationRepository.GetByIdAsync(request.InvitationId);

        if (maybeInvitation.HasNoValue)
        {
            return Result.Failure(DomainErrors.Invitation.NotFound);
        }

        Invitation invitation = maybeInvitation.Value;

        if (invitation.UserId != _userIdentifierProvider.UserId)
        {
            return Result.Failure(DomainErrors.User.InvalidPermissions);
        }

        Result result = invitation.Accept(_dateTime.UtcNow);

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();

    }
}
