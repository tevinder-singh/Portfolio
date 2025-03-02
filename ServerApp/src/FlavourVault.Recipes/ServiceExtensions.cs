using FlavourVault.OutboxDispatcher;
using FlavourVault.SharedCore.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlavourVault.Recipes;

public static class ServiceExtensions
{
    public static IServiceCollection AddRecipesDomainServices(this IServiceCollection services, 
        IConfiguration config,
        List<System.Reflection.Assembly> assemblies)
    {
        services.AddEndpoints(typeof(ServiceExtensions).Assembly);

        // Add services to the container.        
        services.AddDatabaseContext<RecipesDbContext>(config, RecipesDbContext.SchemaName);
        services.AddDatabaseContext<RecipesOutboxDbContext>(config, RecipesDbContext.SchemaName);

        services.AddScoped<IRecipiesUnitOfWork, RecipiesUnitOfWork>();                
        services.AddScoped<IRecipesRepository, RecipesRepository>();        
        
        //backgroud service to send messages async
        services.AddHostedService<OutboxDispatcherService<RecipesOutboxDbContext>>();

        assemblies.Add(typeof(ServiceExtensions).Assembly);
        assemblies.Add(typeof(CreateRecipeRequest).Assembly);
        return services;
    }
}