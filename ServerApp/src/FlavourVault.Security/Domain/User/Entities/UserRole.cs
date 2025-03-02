using FlavourVault.SharedCore.Domain.Common;

namespace FlavourVault.Security.Domain.User.Entities;

public class UserRole: Entity
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}