using Application.Core.Abstractions.Authentication;
using Application.Core.Abstractions.Common;
using Application.Core.Abstractions.Data;
using Application.Core.Abstractions.Messaging;
using Domain.Core.Errors;
using Domain.Core.Primitives.Maybe;
using Domain.Core.Primitives.Result;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Invitations.Commands.RejectInvitation
{
    internal sealed class RejectInvitationCommandHandler : ICommandHandler<RejectInvitationCommand, Result>
    {
        private readonly IUserIdentifierProvider _userIdentifierProvider;
        private readonly IInvitationRepository _invitationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDateTime _dateTime;

        public RejectInvitationCommandHandler(
            IUserIdentifierProvider userIdentifierProvider,
            IInvitationRepository invitationRepository,
            IUnitOfWork unitOfWork,
            IDateTime dateTime)
        {
            _userIdentifierProvider = userIdentifierProvider;
            _invitationRepository = invitationRepository;
            _unitOfWork = unitOfWork;
            _dateTime = dateTime;
        }

        public async Task<Result> Handle(RejectInvitationCommand request, CancellationToken cancellationToken)
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

            Result rejectResult = invitation.Reject(_dateTime.UtcNow);

            if (rejectResult.IsFailure)
            {
                return Result.Failure(rejectResult.Error);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}