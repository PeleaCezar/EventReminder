﻿using Application.Core.Abstractions.Authentication;
using Application.Core.Abstractions.Messaging;
using Contracts.Authentication;
using Domain.Core.Errors;
using Domain.Core.Primitives.Maybe;
using Domain.Core.Primitives.Result;
using Domain.Entities;
using Domain.Repositories;
using Domain.Services;
using Domain.ValueObjects;

namespace Application.Authentication.Commands.Login
{
    /// <summary>
    /// Represents the <see cref="LoginCommand"/> handler.
    /// </summary>
    internal sealed class LoginCommandHandler : ICommandHandler<LoginCommand, Result<TokenResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashChecker _passwordHashChecker;
        private readonly IJwtProvider _jwtProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginCommandHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="passwordHashChecker">The password hash checker.</param>
        /// <param name="jwtProvider">The JWT provider.</param>
        public LoginCommandHandler(IUserRepository userRepository, IPasswordHashChecker passwordHashChecker, IJwtProvider jwtProvider)
        {
            _userRepository = userRepository;
            _passwordHashChecker = passwordHashChecker;
            _jwtProvider = jwtProvider;
        }

        public async Task<Result<TokenResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            Result<Email> emailResult = Email.Create(request.Email);

            if (emailResult.IsFailure)
            {
                return Result.Failure<TokenResponse>(DomainErrors.Authentication.InvalidEmailOrPassword);
            }

            Maybe<User> maybeUser = await _userRepository.GetByEmailAsync(emailResult.Value);

            if (maybeUser.HasNoValue)
            {
                return Result.Failure<TokenResponse>(DomainErrors.Authentication.InvalidEmailOrPassword);
            }

            User user = maybeUser.Value;

            bool passwordValid = user.VerifyPasswordHash(request.Password, _passwordHashChecker);

            if (!passwordValid)
            {
                return Result.Failure<TokenResponse>(DomainErrors.Authentication.InvalidEmailOrPassword);
            }

            string token = _jwtProvider.Create(user);

            return Result.Success(new TokenResponse(token));
        }
    }
}
