﻿using Application.Core.Abstractions.Authentication;
using Application.Core.Abstractions.Common;
using Application.Core.Abstractions.Data;
using Application.Core.Abstractions.Messaging;
using Domain.Core.Errors;
using Domain.Core.Primitives.Maybe;
using Domain.Core.Primitives.Result;
using Domain.Entities;
using Domain.Repositories;

namespace Application.PersonalEvents.Commands.DeletePersonalEvent;

/// <summary>
/// Represents the <see cref="CancelPersonalEventCommand"/> handler.
/// </summary>
internal sealed class CancelPersonalEventCommandHandler : ICommandHandler<CancelPersonalEventCommand, Result>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IPersonalEventRepository _personalEventRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTime _dateTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="CancelPersonalEventCommandHandler"/> class.
    /// </summary>
    /// <param name="userIdentifierProvider">The user identifier provider.</param>
    /// <param name="personalEventRepository">The personal event repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="dateTime">The date and time.</param>
    public CancelPersonalEventCommandHandler(
        IUserIdentifierProvider userIdentifierProvider,
        IPersonalEventRepository personalEventRepository,
        IUnitOfWork unitOfWork,
        IDateTime dateTime)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _personalEventRepository = personalEventRepository;
        _unitOfWork = unitOfWork;
        _dateTime = dateTime;
    }

    public async Task<Result> Handle(CancelPersonalEventCommand request, CancellationToken cancellationToken)
    {
        Maybe<PersonalEvent> maybePersonalEvent = await _personalEventRepository.GetByIdAsync(request.PersonalEventId);

        if (maybePersonalEvent.HasNoValue)
        {
            return Result.Failure(DomainErrors.PersonalEvent.NotFound);
        }

        PersonalEvent personalEvent = maybePersonalEvent.Value;

        if (personalEvent.UserId != _userIdentifierProvider.UserId)
        {
            return Result.Failure(DomainErrors.User.InvalidPermissions);
        }

        Result result = personalEvent.Cancel(_dateTime.UtcNow);

        if (result.IsFailure)
        {
            return result;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
