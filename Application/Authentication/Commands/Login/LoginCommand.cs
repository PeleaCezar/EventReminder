using Application.Core.Abstractions.Messaging;
using Contracts.Authentication;
using Domain.Core.Primitives.Result;

namespace Application.Authentication.Commands.Login;

public sealed class LoginCommand : ICommand<Result<TokenResponse>>
{
    public LoginCommand(string email, string password)
    {
        Email = email;
        Password = password;
    }

    public string Email { get; }

    public string Password { get; }
}
