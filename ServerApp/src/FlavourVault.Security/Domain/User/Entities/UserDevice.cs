using FlavourVault.SharedCore.Domain.Common;

namespace FlavourVault.Security.Domain.User.Entities;

internal sealed class UserDevice : Entity
{
    public Guid UserId { get; set; }
    public string DeviceId { get; set; }
    public string DeviceName { get; set; }
    public string DeviceType { get; set; }
    public string OS { get; set; } 
    public string Browser { get; set; }
    public string IPAddress { get; set; }
    public bool IsTrusted { get; set; }
    public DateTime LastLoginDate { get; set; }
}
