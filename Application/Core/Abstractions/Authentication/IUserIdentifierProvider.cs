namespace Application.Core.Abstractions.Authentication;

public interface IUserIdentifierProvider
{
    /// <summary>
    /// Gets the authenticated user identifier.
    /// </summary>
    Guid UserId { get; }
}
