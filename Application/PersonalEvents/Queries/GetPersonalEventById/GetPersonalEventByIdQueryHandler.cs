using Application.Core.Abstractions.Authentication;
using Application.Core.Abstractions.Data;
using Application.Core.Abstractions.Messaging;
using Contracts.PersonalEvents;
using Domain.Core.Primitives.Maybe;
using Domain.Entities;
using Domain.Enumerations;
using Microsoft.EntityFrameworkCore;
namespace Application.PersonalEvents.Queries.GetPersonalEventById
{
    /// <summary>
    /// Represents the <see cref="GetPersonalEventByIdQuery"/> handler.
    /// </summary>
    internal sealed class GetPersonalEventByIdQueryHandler : IQueryHandler<GetPersonalEventByIdQuery, Maybe<DetailedPersonalEventResponse>>
    {
        private readonly IDbContext _dbContext;
        private readonly IUserIdentifierProvider _userIdentifierProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetPersonalEventByIdQueryHandler"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="userIdentifierProvider">The user identifier provider.</param>
        public GetPersonalEventByIdQueryHandler(IDbContext dbContext, IUserIdentifierProvider userIdentifierProvider)
        {
            _dbContext = dbContext;
            _userIdentifierProvider = userIdentifierProvider;
        }

        /// <inheritdoc />
        public async Task<Maybe<DetailedPersonalEventResponse>> Handle(
            GetPersonalEventByIdQuery request,
            CancellationToken cancellationToken)
        {
            if (request.PersonalEventId == Guid.Empty)
            {
                return Maybe<DetailedPersonalEventResponse>.None;
            }

            DetailedPersonalEventResponse response = await (
                    from personalEvent in _dbContext.Set<PersonalEvent>().AsNoTracking()
                    join user in _dbContext.Set<User>().AsNoTracking()
                        on personalEvent.UserId equals user.Id
                    where user.Id == _userIdentifierProvider.UserId &&
                          personalEvent.Id == request.PersonalEventId &&
                          !personalEvent.Cancelled
                    select new DetailedPersonalEventResponse
                    {
                        Id = personalEvent.Id,
                        Name = personalEvent.Name.Value,
                        CategoryId = personalEvent.Category.Value,
                        CreatedBy = user.FirstName.Value + " " + user.LastName.Value,
                        DateTimeUtc = personalEvent.DateTimeUtc,
                        CreatedOnUtc = personalEvent.CreatedOnUtc
                    }).FirstOrDefaultAsync(cancellationToken);

            if (response is null)
            {
                return Maybe<DetailedPersonalEventResponse>.None;
            }

            response.Category = Category.FromValue(response.CategoryId).Value.Name;

            return response;
        }
    }
}
