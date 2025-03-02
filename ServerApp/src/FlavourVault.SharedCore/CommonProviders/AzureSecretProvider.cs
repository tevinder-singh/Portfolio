using FlavourVault.SharedCore.Interfaces;

namespace FlavourVault.SharedCore.CommonProviders;
public sealed class AzureSecretProvider : ISecretProvider
{
    public async Task<string> GetAsync(string secretName)
    {
        throw new NotImplementedException();
    }
}