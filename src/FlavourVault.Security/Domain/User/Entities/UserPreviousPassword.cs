using FlavourVault.SharedCore.Domain.Common;

namespace FlavourVault.Security.Domain.User.Entities;

internal sealed class UserPreviousPassword : Entity
{
    public Guid UserId { get; set; }
    public string PasswordHash { get; set; }
    public string Salt { get; set; }
}