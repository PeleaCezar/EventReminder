using Application.Core.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Authentication;

internal sealed class UserIdentifierProvider : IUserIdentifierProvider
{
	public UserIdentifierProvider(IHttpContextAccessor httpContextAccessor)
	{
        string userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirstValue("userId")
               ?? throw new ArgumentException("The user identifier claim is required.", nameof(httpContextAccessor));

        UserId = new Guid(userIdClaim);
    }

    public Guid UserId { get; }
}
