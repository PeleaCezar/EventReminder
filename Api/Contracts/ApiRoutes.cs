namespace Api.Contracts;

/// <summary>
/// Contains the API endpoint routes.
/// </summary>
/// 
public static class ApiRoutes
{
    public static class Authentication
    {
        public const string Login = "authentication/login";

        public const string Register = "authentication/register";
    }

    public static class Attendees
    {
        public const string Get = "attendees";
    }

    public static class FriendshipRequests
    {
        public const string GetById = "friendship-requests/{friendshipRequestId:guid}";

        public const string GetPending = "friendship-requests/pending";

        public const string GetSent = "friendship-requests/sent";

        public const string Accept = "friendship-requests/{friendshipRequestId:guid}/accept";

        public const string Reject = "friendship-requests/{friendshipRequestId:guid}/reject";
    }

    public static class Friendships
    {
        public const string Get = "friendships/{userId:guid}/{friendId:guid}";

        public const string GetForUserId = "friendships/{userId:guid}";

        public const string Remove = "friendships/{userId:guid}/{friendId:guid}";
    }
}
