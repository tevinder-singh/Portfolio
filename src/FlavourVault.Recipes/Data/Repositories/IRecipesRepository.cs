using FlavourVault.Recipes.Domain.Recipe;

namespace FlavourVault.Recipes.Data.Repositories;
internal interface IRecipesRepository
{
    void AddRecipe(Recipe recipe);
}
