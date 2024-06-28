using Application.Core.Abstractions.Data;
using Domain.Core.Primitives.Maybe;
using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using Persistence.Specifications;

namespace Persistence.Repositories;

internal sealed class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(IDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<Maybe<User>> GetByEmailAsync(Email email) => await FirstOrDefaultAsync(new UserWithEmailSpecification(email));

    public async Task<bool> IsEmailUniqueAsync(Email email) => !await AnyAsync(new UserWithEmailSpecification(email));
}
