﻿using Application.Core.Abstractions.Authentication;
using Application.Core.Abstractions.Common;
using Application.Core.Abstractions.Data;
using Application.Core.Abstractions.Messaging;
using Domain.Core.Errors;
using Domain.Core.Primitives.Maybe;
using Domain.Core.Primitives.Result;
using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;

namespace Application.PersonalEvents.Commands.UpdatePersonalEvent;

/// <summary>
/// Represents the <see cref="UpdatePersonalEventCommand"/> handler.
/// </summary>
internal sealed class UpdatePersonalEventCommandHandler : ICommandHandler<UpdatePersonalEventCommand, Result>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IPersonalEventRepository _personalEventRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTime _dateTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdatePersonalEventCommandHandler"/> class.
    /// </summary>
    /// <param name="userIdentifierProvider">The user identifier provider.</param>
    /// <param name="personalEventRepository">The personal event repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="dateTime">The date and time.</param>
    public UpdatePersonalEventCommandHandler(
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

    public async Task<Result> Handle(UpdatePersonalEventCommand request, CancellationToken cancellationToken)
    {
       if(request.DateTimeUtc <= _dateTime.UtcNow)
       {
           return Result.Failure(DomainErrors.PersonalEvent.DateAndTimeIsInThePast);
       }

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

       Result<Name> nameResult = Name.Create(request.Name);

       if (nameResult.IsFailure)
       {
          return Result.Failure(nameResult.Error);
       }

       personalEvent.ChangeName(nameResult.Value);

       personalEvent.ChangeDateAndTime(request.DateTimeUtc);

       await _unitOfWork.SaveChangesAsync(cancellationToken);

       return Result.Success();
    }
}
