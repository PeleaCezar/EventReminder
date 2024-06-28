using Domain.Core.Abstractions;
using Domain.Core.Errors;
using Domain.Core.Primitives;
using Domain.Core.Primitives.Result;
using Domain.Core.Utility;
using Domain.Events;
using Domain.Repositories;
using Domain.Services;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public sealed class User : AggregateRoot, IAuditableEntity, ISoftDeletableEntity
    {
        private string _passwordHash;

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="firstName">The user first name.</param>
        /// <param name="lastName">The user last name.</param>
        /// <param name="email">The user email instance.</param>
        /// <param name="passwordHash">The user password hash.</param>
        private User(FirstName firstName, LastName lastName, Email email, string passwordHash)
            : base(Guid.NewGuid())
        {
            Ensure.NotEmpty(firstName, "The first name is required.", nameof(firstName));
            Ensure.NotEmpty(lastName, "The last name is required.", nameof(lastName));
            Ensure.NotEmpty(email, "The email is required.", nameof(email));
            Ensure.NotEmpty(passwordHash, "The password hash is required", nameof(passwordHash));

            FirstName = firstName;
            LastName = lastName;
            Email = email;
            _passwordHash = passwordHash;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <remarks>
        /// Required by EF Core.
        /// </remarks>
        private User()
        {
        }

        /// <summary>
        /// Gets the user first name.
        /// </summary>
        public FirstName FirstName { get; private set; }

        /// <summary>
        /// Gets the user last name.
        /// </summary>
        public LastName LastName { get; private set; }

        /// <summary>
        /// Gets the user full name.
        /// </summary>
        public string FullName => $"{FirstName} {LastName}";

        public Email Email { get; private set; }

        public DateTime CreatedOnUtc { get; }

        public DateTime? ModifiedOnUtc { get; }

        public DateTime? DeletedOnUtc { get; }

        public bool Deleted { get; }

        public static User Create(FirstName firstName, LastName lastName, Email email, string passwordHash)
        {
            var user = new User(firstName, lastName, email, passwordHash);

            user.AddDomainEvent(new UserCreatedDomainEvent(user));

            return user;
        }

        /// <summary>
        /// Creates a new friendship request for the specified friend.
        /// </summary>
        /// <param name="friend">The friend.</param>
        /// <param name="friendshipRepository">The friendship repository.</param>
        /// <param name="friendshipRequestRepository">The friendship request repository.</param>
        /// <returns>The result that contains a friendship request or an error.</returns>
        public async Task<Result<FriendshipRequest>> SendFriendshipRequestAsync(
            User friend,
            IFriendshipRepository friendshipRepository,
            IFriendshipRequestRepository friendshipRequestRepository)
        {
            if (await friendshipRepository.CheckIfFriendsAsync(this, friend))
            {
                return Result.Failure<FriendshipRequest>(DomainErrors.FriendshipRequest.AlreadyFriends);
            }

            if (await friendshipRequestRepository.CheckForPendingFriendshipRequestAsync(this, friend))
            {
                return Result.Failure<FriendshipRequest>(DomainErrors.FriendshipRequest.PendingFriendshipRequest);
            }

            var friendshipRequest = new FriendshipRequest(this, friend);

            AddDomainEvent(new FriendshipRequestSentDomainEvent(friendshipRequest));

            return friendshipRequest;
        }


        /// <summary>
        /// Verifies that the provided password hash matches the password hash.
        /// </summary>
        /// <param name="password">The password to be checked against the user password hash.</param>
        /// <param name="passwordHashChecker">The password hash checker.</param>
        /// <returns>True if the password hashes match, otherwise false.</returns>
        public bool VerifyPasswordHash(string password, IPasswordHashChecker passwordHashChecker)
            => !string.IsNullOrWhiteSpace(password) && passwordHashChecker.HashesMatch(_passwordHash, password);


        /// <summary>
        /// Changes the users password.
        /// </summary>
        /// <param name="passwordHash">The password hash of the new password.</param>
        /// <returns>The success result or an error.</returns>
        public Result ChangePassword(string passwordHash)
        {
            if (passwordHash == _passwordHash)
            {
                return Result.Failure(DomainErrors.User.CannotChangePassword);
            }

            _passwordHash = passwordHash;

            AddDomainEvent(new UserPasswordChangedDomainEvent(this));

            return Result.Success();
        }


        /// <summary>
        /// Changes the users first and last name.
        /// </summary>
        /// <param name="firstName">The new first name.</param>
        /// <param name="lastName">The new last name.</param>
        public void ChangeName(FirstName firstName, LastName lastName)
        {
            Ensure.NotEmpty(firstName, "The first name is required.", nameof(firstName));
            Ensure.NotEmpty(lastName, "The last name is required.", nameof(lastName));

            FirstName = firstName;

            LastName = lastName;

            AddDomainEvent(new UserNameChangedDomainEvent(this));
        }

        /// <summary>
        /// Removes the specified friendship.
        /// </summary>
        /// <param name="friendship">The friendship.</param>
        internal void RemoveFriendship(Friendship friendship) => AddDomainEvent(new FriendshipRemovedDomainEvent(friendship));
    }
}
