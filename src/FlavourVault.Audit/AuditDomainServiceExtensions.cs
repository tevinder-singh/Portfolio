using FlavourVault.Recipes.Data;
using FlavourVault.SharedCore.Data;
using FlavourVault.SharedCore.Extensions;

namespace FlavourVault.Security;

public static class AuditDomainServiceExtensions
{
    public static IServiceCollection AddAuditDomainServices(this IServiceCollection services, 
        ConfigurationManager config,
        List<System.Reflection.Assembly> assemblies)
    {
        services.AddEndpoints(typeof(AuditDomainServiceExtensions).Assembly);

        // Add services to the container.        
        services.AddDatabaseContext<AuditDbContext>(config, AuditDbContext.SchemaName);
                                
        //services.AddScoped<IUsersRepository, UsersRepository>();        
        
        assemblies.Add(typeof(AuditDomainServiceExtensions).Assembly);
        //assemblies.Add(typeof(AuthenticateUserRequest).Assembly);
        return services;
    }
}