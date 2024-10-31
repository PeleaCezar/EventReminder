using Application.Core.Abstractions.Authentication;
using Application.Core.Abstractions.Data;
using Application.Core.Abstractions.Messaging;
using Contracts.Users;
using Domain.Core.Primitives.Maybe;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Query.GetUserById;

internal sealed class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, Maybe<UserResponse>>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="userIdentifierProvider">The user identifier provider.</param>
    /// <param name="dbContext">The database context.</param>
    public GetUserByIdQueryHandler(IUserIdentifierProvider userIdentifierProvider, IDbContext dbContext)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _dbContext = dbContext;
    }

    public async Task<Maybe<UserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.UserId == Guid.Empty || request.UserId != _userIdentifierProvider.UserId)
        {
            return Maybe<UserResponse>.None;
        }

        UserResponse response = await _dbContext.Set<User>()
            .Where(x => x.Id == request.UserId)
            .Select(user => new UserResponse
            {
                Id = user.Id,
                Email = user.Email.Value,
                FullName = user.FirstName.Value + " " + user.LastName.Value,
                FirstName = user.FirstName.Value,
                LastName = user.LastName.Value,
                CreatedOnUtc = user.CreatedOnUtc
            })
            .SingleOrDefaultAsync(cancellationToken);

        if(response is null)
        {
            return Maybe<UserResponse>.None;
        }

        response.NumberOfFriends = await _dbContext.Set<Friendship>().CountAsync(x => x.UserId == response.Id, cancellationToken);

        response.NumberOfPersonalEvents = await _dbContext.Set<PersonalEvent>()
            .CountAsync(x => x.UserId == response.Id && !x.Cancelled, cancellationToken);

        return response;
    }
}
