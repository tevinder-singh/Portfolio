using FlavourVault.Security.Contracts;
using FlavourVault.Security.Data;
using FlavourVault.Security.Data.Repositories;
using FlavourVault.SharedCore.Data;
using FlavourVault.SharedCore.Extensions;

namespace FlavourVault.Security;

public static class SecurityDomainServiceExtensions
{
    public static IServiceCollection AddSecurityDomainServices(this IServiceCollection services, 
        ConfigurationManager config,
        List<System.Reflection.Assembly> assemblies)
    {
        services.AddEndpoints(typeof(SecurityDomainServiceExtensions).Assembly);

        // Add services to the container.        
        services.AddDatabaseContext<SecurityDbContext>(config, SecurityDbContext.SchemaName);

        services.AddScoped<ISecurityUnitOfWork, SecurityUnitOfWork>();
        //services.AddScoped<IOutboxRepository<SecurityDbContext>, OutboxRepository<SecurityDbContext>>();
                
        services.AddScoped<IUsersRepository, UsersRepository>();        
        //services.AddSingleton<IOutboxProcessor, OutboxProcessor>();

        ////backgroud service to send messages async
        //services.AddHostedService<OutboxBackgroundService>();

        assemblies.Add(typeof(SecurityDomainServiceExtensions).Assembly);
        assemblies.Add(typeof(AuthenticateUserRequest).Assembly);
        return services;
    }
}