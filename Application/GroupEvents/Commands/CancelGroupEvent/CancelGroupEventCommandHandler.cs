﻿using Application.Core.Abstractions.Authentication;
using Application.Core.Abstractions.Common;
using Application.Core.Abstractions.Data;
using Application.Core.Abstractions.Messaging;
using Domain.Core.Errors;
using Domain.Core.Primitives.Maybe;
using Domain.Core.Primitives.Result;
using Domain.Entities;
using Domain.Repositories;

namespace Application.GroupEvents.Commands.CancelGroupEvent;

internal sealed class CancelGroupEventCommandHandler : ICommandHandler<CancelGroupEventCommand, Result>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IGroupEventRepository _groupEventRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTime _dateTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="CancelGroupEventCommandHandler"/> class.
    /// </summary>
    /// <param name="userIdentifierProvider">The user identifier provider.</param>
    /// <param name="groupEventRepository">The group event repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="dateTime">The date and time.</param>
    public CancelGroupEventCommandHandler(
        IUserIdentifierProvider userIdentifierProvider,
        IGroupEventRepository groupEventRepository,
        IUnitOfWork unitOfWork,
        IDateTime dateTime)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _groupEventRepository = groupEventRepository;
        _unitOfWork = unitOfWork;
        _dateTime = dateTime;
    }

    public async Task<Result> Handle(CancelGroupEventCommand request, CancellationToken cancellationToken)
    {
        Maybe<GroupEvent> maybeGroupEvent = await _groupEventRepository.GetByIdAsync(request.GroupEventId);

        if(maybeGroupEvent.HasNoValue)
        {
            return Result.Failure(DomainErrors.GroupEvent.NotFound);
        }

        GroupEvent groupEvent = maybeGroupEvent.Value;

        if(groupEvent.UserId != _userIdentifierProvider.UserId)
        {
            return Result.Failure(DomainErrors.User.InvalidPermissions);
        }

        Result result = groupEvent.Cancel(_dateTime.UtcNow);

        if(result.IsFailure)
        {
            return result;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
