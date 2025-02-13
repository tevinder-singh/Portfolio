using FlavourVault.Recipes.Domain.Recipe;

namespace FlavourVault.Recipes.Data.Repositories;
internal sealed class RecipesRepository(RecipesDbContext context) : IRecipesRepository
{
    public void AddRecipe(Recipe recipe)
    {
        context.Add(recipe);
    }
}
