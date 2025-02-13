using FlavourVault.SharedCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace FlavourVault.SharedCore.Domain.Common;
public sealed class UserContext : IUserContext
{
    private readonly Guid? _currentUserId;
    private readonly string? _currentUserName;

    public UserContext(IHttpContextAccessor accessor)
    {
        var userId = accessor.HttpContext?.User.FindFirstValue("userid");
        if (userId is not null)        
            _currentUserId = Guid.TryParse(userId, out var guid) ? guid : null;

        var userName = accessor.HttpContext?.User?.Identity?.Name;
        if (userName is not null)
            _currentUserName = userName;
    }

    public Guid? GetUserId() => _currentUserId;
    public string? GetUserName() => _currentUserName;
}