using Application.Core.Abstractions.Authentication;
using Application.Core.Abstractions.Common;
using Application.Core.Abstractions.Data;
using Application.Core.Abstractions.Messaging;
using Domain.Core.Errors;
using Domain.Core.Primitives.Maybe;
using Domain.Core.Primitives.Result;
using Domain.Entities;
using Domain.Repositories;

namespace Application.FriendshipRequests.Commands.RejectFriendshipRequest
{
    internal sealed class RejectFriendshipRequestCommandHandler : ICommandHandler<RejectFriendshipRequestCommand, Result>
    {
        private readonly IUserIdentifierProvider _userIdentifierProvider;
        private readonly IFriendshipRequestRepository _friendshipRequestRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDateTime _dateTime;

        public RejectFriendshipRequestCommandHandler(
            IUserIdentifierProvider userIdentifierProvider,
            IFriendshipRequestRepository friendshipRequestRepository,
            IUnitOfWork unitOfWork,
            IDateTime dateTime)
        {
            _userIdentifierProvider = userIdentifierProvider;
            _friendshipRequestRepository = friendshipRequestRepository;
            _unitOfWork = unitOfWork;
            _dateTime = dateTime;
        }

        public async Task<Result> Handle(RejectFriendshipRequestCommand request, CancellationToken cancellationToken)
        {
            Maybe<FriendshipRequest> maybeFriendshipRequest = await _friendshipRequestRepository.GetByIdAsync(request.FriendshipRequestId);

            if (maybeFriendshipRequest.HasNoValue)
            {
                return Result.Failure(DomainErrors.FriendshipRequest.NotFound);
            }

            FriendshipRequest friendshipRequest = maybeFriendshipRequest.Value;

            if (friendshipRequest.FriendId != _userIdentifierProvider.UserId)
            {
                return Result.Failure(DomainErrors.User.InvalidPermissions);
            }

            Result rejectResult = friendshipRequest.Reject(_dateTime.UtcNow);

            if (rejectResult.IsFailure)
            {
                return Result.Failure(rejectResult.Error);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
