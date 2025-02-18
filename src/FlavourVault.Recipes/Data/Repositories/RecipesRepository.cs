using FlavourVault.Recipes.Domain.Recipe;

namespace FlavourVault.Recipes.Data.Repositories;
internal sealed class RecipesRepository(RecipesDbContext context) : IRecipesRepository
{
    public async Task<Recipe?> GetRecipeByIdAsync(Guid id)
    {
        return await context.Recipes.FindAsync(id);
    }

    public void AddRecipe(Recipe recipe)
    {
        context.Add(recipe);
    }

    public void UpdateRecipe(Recipe recipe)
    {
        context.Update(recipe);
    }
}
