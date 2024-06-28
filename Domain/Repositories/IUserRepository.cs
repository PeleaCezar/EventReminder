using Domain.Core.Primitives.Maybe;
using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Repositories;

public interface IUserRepository
{
    Task<Maybe<User>> GetByIdAsync(Guid userId);

    Task<Maybe<User>> GetByEmailAsync(Email email);

    Task<bool> IsEmailUniqueAsync(Email email);

    void Insert(User user);
}
