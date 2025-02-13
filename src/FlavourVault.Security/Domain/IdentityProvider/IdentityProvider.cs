using FlavourVault.SharedCore.Domain.Common;

namespace FlavourVault.Security.Domain.IdentityProvider;

internal sealed class IdentityProvider : AggregateRoot
{
    public string Name { get; set; }
}
