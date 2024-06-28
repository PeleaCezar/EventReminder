namespace Contracts.Authentication;

public sealed class TokenResponse
{
    public TokenResponse(string token) => Token = token;

    /// <summary>
    /// Gets the token.
    /// </summary>
    public string Token { get; }
}
