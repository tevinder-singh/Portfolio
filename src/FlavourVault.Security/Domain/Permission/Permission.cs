using FlavourVault.SharedCore.Domain.Common;

namespace FlavourVault.Security.Domain.Permission;

internal sealed class Permission : AggregateRoot
{
    public Permission() { }
    public Permission(string name, string resource, string action)
    {
        Name = name;
        Resource = resource;
        Action = action;
        Code = $"{resource}:{action}";
    }

    public string Name { get; private set; }

    public string Code { get; private set; }

    /// <summary>
    /// Define what permission applied to
    /// </summary>
    public string Resource { get; private set; }

    /// <summary>
    /// Define action e.g. Get, List, Delete
    /// </summary>
    public string Action { get; private set; }
}