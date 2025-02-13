using FlavourVault.Recipes.BackgroundServices;
using FlavourVault.Recipes.Contracts;
using FlavourVault.Recipes.Data;
using FlavourVault.Recipes.Data.Repositories;
using FlavourVault.SharedCore.Data;
using FlavourVault.SharedCore.Extensions;

namespace FlavourVault.Recipes;

public static class RecipesDomainServiceExtensions
{
    public static IServiceCollection AddRecipesDomainServices(this IServiceCollection services, 
        IConfiguration config,
        List<System.Reflection.Assembly> assemblies)
    {
        services.AddEndpoints(typeof(RecipesDomainServiceExtensions).Assembly);

        // Add services to the container.        
        services.AddDatabaseContext<RecipesDbContext>(config, RecipesDbContext.SchemaName);
                
        services.AddScoped<IRecipiesUnitOfWork, RecipiesUnitOfWork>();        
        //services.AddScoped<IOutboxRepository<RecipesDbContext>, OutboxRepository<RecipesDbContext>>();
        services.AddScoped<IRecipesRepository, RecipesRepository>();        
        services.AddSingleton<IOutboxProcessor, OutboxProcessor>();

        //backgroud service to send messages async
        services.AddHostedService<OutboxBackgroundService>();

        assemblies.Add(typeof(RecipesDomainServiceExtensions).Assembly);
        assemblies.Add(typeof(CreateRecipeRequest).Assembly);
        return services;
    }
}