using FlavourVault.SharedCore.Domain.Common;

namespace FlavourVault.Security.Domain.Role.Entities;

internal sealed class RolePermission : Entity
{
    public RolePermission() { }

    public RolePermission(Guid roleId, Guid permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }

    public Guid RoleId { get; private set; }
    public Guid PermissionId { get; private set; }
}