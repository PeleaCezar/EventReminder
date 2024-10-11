using Application.Core.Abstractions.Authentication;
using Application.Core.Abstractions.Common;
using Application.Core.Abstractions.Data;
using Application.Core.Abstractions.Messaging;
using Domain.Core.Errors;
using Domain.Core.Primitives.Maybe;
using Domain.Core.Primitives.Result;
using Domain.Entities;
using Domain.Enumerations;
using Domain.Repositories;
using Domain.ValueObjects;

namespace Application.GroupEvents.Commands.CreateGroupEvent;

internal sealed class CreateGroupEventCommandHandler : ICommandHandler<CreateGroupEventCommand, Result>
{

    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IGroupEventRepository _groupEventRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTime _dateTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateGroupEventCommandHandler"/> class.
    /// </summary>
    /// <param name="userIdentifierProvider">The user identifier provider.</param>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="groupEventRepository">The group event repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="dateTime">The date and time.</param>
    /// <param name="mediator">The mediator.</param>
    public CreateGroupEventCommandHandler(
        IUserIdentifierProvider userIdentifierProvider,
        IUserRepository userRepository,
        IGroupEventRepository groupEventRepository,
        IUnitOfWork unitOfWork,
        IDateTime dateTime)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _userRepository = userRepository;
        _groupEventRepository = groupEventRepository;
        _unitOfWork = unitOfWork;
        _dateTime = dateTime;
    }

    public async Task<Result> Handle(CreateGroupEventCommand request, CancellationToken cancellationToken)
    {
        if(request.UserId != _userIdentifierProvider.UserId)
        {
            return Result.Failure(DomainErrors.User.InvalidPermissions);
        }

        if(request.DateTimeUtc <= _dateTime.UtcNow)
        {
            return Result.Failure(DomainErrors.GroupEvent.DateAndTimeIsInThePast);
        }

        Maybe<User> maybeUser = await _userRepository.GetByIdAsync(request.UserId);

        if(maybeUser.HasNoValue)
        {
            return Result.Failure(DomainErrors.User.NotFound);
        }

        Maybe<Category> maybeCategory = Category.FromValue(request.CategoryId);

        if(maybeCategory.HasNoValue)
        {
            return Result.Failure(DomainErrors.Category.NotFound);
        }

        Result<Name> nameResult = Name.Create(request.Name);

        if(nameResult.IsFailure)
        {
            return Result.Failure(nameResult.Error);
        }

        var groupEvent = GroupEvent.Create(maybeUser.Value, nameResult.Value, maybeCategory.Value, request.DateTimeUtc);

        _groupEventRepository.Insert(groupEvent);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
