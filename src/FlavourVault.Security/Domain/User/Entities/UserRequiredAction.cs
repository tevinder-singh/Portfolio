using FlavourVault.SharedCore.Domain.Common;
using FlavourVault.Security.Domain.User.Enums;

namespace FlavourVault.Security.Domain.User.Entities;

internal sealed class UserRequiredAction : Entity
{
    public Guid UserId { get; set; }
    public RequiredAction RequiredAction { get; set; }
}
