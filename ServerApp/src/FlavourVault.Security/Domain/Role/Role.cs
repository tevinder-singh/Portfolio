using FlavourVault.SharedCore.Domain.Common;
using FlavourVault.Security.Domain.Role.Entities;

namespace FlavourVault.Security.Domain.Role;

internal sealed class Role : AggregateRoot
{
    public Role() { }

    public Role(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public ICollection<RolePermission> Permissions { get; private set; } = [];

    public void AddPermission(RolePermission permission)
    {
        Permissions.Add(permission);
    }
}
