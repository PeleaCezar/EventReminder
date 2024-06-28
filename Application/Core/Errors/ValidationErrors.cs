using Domain.Core.Primitives;

namespace Application.Core.Errors;

internal static class ValidationErrors
{
    internal static class Login
    {
        internal static Error EmailIsRequired => new Error("Login.EmailIsRequired", "The email is required.");

        internal static Error PasswordIsRequired => new Error("Login.PasswordIsRequired", "The password is required.");
    }

    internal static class RejectFriendshipRequest
    {
        internal static Error FriendshipRequestIdIsRequired => new Error(
            "RejectFriendshipRequest.FriendshipRequestIdIsRequired",
            "The invitation identifier is required.");
    }

    internal static class CreateUser
    {
        internal static Error FirstNameIsRequired => new Error("CreateUser.FirstNameIsRequired", "The first name is required.");

        internal static Error LastNameIsRequired => new Error("CreateUser.LastNameIsRequired", "The last name is required.");

        internal static Error EmailIsRequired => new Error("CreateUser.EmailIsRequired", "The email is required.");

        internal static Error PasswordIsRequired => new Error("CreateUser.PasswordIsRequired", "The password is required.");
    }

    internal static class AcceptFriendshipRequest
    {
        internal static Error FriendshipRequestIdIsRequired => new Error(
            "AcceptFriendshipRequest.FriendshipRequestIdIsRequired",
            "The invitation identifier is required.");
    }
}
