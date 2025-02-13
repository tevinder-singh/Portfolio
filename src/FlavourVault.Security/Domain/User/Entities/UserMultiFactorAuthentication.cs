using FlavourVault.SharedCore.Domain.Common;
using FlavourVault.Security.Domain.User.Enums;

namespace FlavourVault.Security.Domain.User.Entities;

internal sealed class UserMultiFactorAuthentication : Entity
{
    public Guid UserId { get; set; }        
    public MultiFactorAuthenticationType Type { get; set; }
    public string Secret { get; set; }
    public bool Enabled { get; set; }
}