using Microsoft.AspNetCore.Routing;

namespace FlavourVault.SharedCore.Interfaces;
public interface IMinimalApiEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
