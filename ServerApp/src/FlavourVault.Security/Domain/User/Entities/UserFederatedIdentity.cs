using FlavourVault.SharedCore.Domain.Common;

namespace FlavourVault.Security.Domain.User.Entities;

internal sealed class UserFederatedIdentity : Entity
{
    public Guid UserId { get; set; }
    public Guid IdentityProviderId { get; set; }
    public string FederatedUsername { get; set; }

}
