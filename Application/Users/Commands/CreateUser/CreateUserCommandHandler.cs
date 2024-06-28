using Application.Core.Abstractions.Authentication;
using Application.Core.Abstractions.Cryptography;
using Application.Core.Abstractions.Data;
using Application.Core.Abstractions.Messaging;
using Contracts.Authentication;
using Domain.Core.Errors;
using Domain.Core.Primitives.Result;
using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;

namespace Application.Users.Commands.CreateUser
{
    internal sealed class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Result<TokenResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtProvider _jwtProvider;

        public CreateUserCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork,
            IJwtProvider jwtProvider)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
            _jwtProvider = jwtProvider;
        }

        public async Task<Result<TokenResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            Result<FirstName> firstNameResult = FirstName.Create(request.FirstName);
            Result<LastName> lastNameResult = LastName.Create(request.LastName);
            Result<Email> emailResult = Email.Create(request.Email);
            Result<Password> passwordResult = Password.Create(request.Password);

            Result firstFailureOrSuccess = Result.FirstFailureOrSuccess(firstNameResult, lastNameResult, emailResult, passwordResult);

            if (firstFailureOrSuccess.IsFailure)
            {
                return Result.Failure<TokenResponse>(firstFailureOrSuccess.Error);
            }

            if (!await _userRepository.IsEmailUniqueAsync(emailResult.Value))
            {
                return Result.Failure<TokenResponse>(DomainErrors.User.DuplicateEmail);
            }

            string passwordHash = _passwordHasher.HashPassword(passwordResult.Value);

            var user = User.Create(firstNameResult.Value, lastNameResult.Value, emailResult.Value, passwordHash);

            _userRepository.Insert(user);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            string token = _jwtProvider.Create(user);

            return Result.Success(new TokenResponse(token));
        }
    }
}
