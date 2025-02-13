using FlavourVault.SharedCore.Domain.Common;
using FlavourVault.Security.Domain.User.Enums;

namespace FlavourVault.Security.Domain.User.Entities;

internal sealed class UserToken : Entity
{
    public Guid UserId { get; set; }
    public string Value { get; set; }
    public TokenType Type { get; set; }
    public DateTime ExpiresOn { get; set; }
    public DateTime? RevokedOn { get; set; }
}