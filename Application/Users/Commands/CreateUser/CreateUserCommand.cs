using Application.Core.Abstractions.Messaging;
using Contracts.Authentication;
using Domain.Core.Primitives.Result;

namespace Application.Users.Commands.CreateUser;

public sealed class CreateUserCommand : ICommand<Result<TokenResponse>>
{
    public CreateUserCommand(string firstName, string lastName, string email, string password)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Password = password;
    }

    public string FirstName { get; }

    public string LastName { get; }

    public string Email { get; }

    public string Password { get; }
}
