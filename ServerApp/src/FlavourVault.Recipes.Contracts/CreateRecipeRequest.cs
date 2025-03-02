namespace FlavourVault.Recipes.Contracts;

public sealed record CreateRecipeRequest(string Name, string Description) : RecipeRequest(Name, Description), IRequest<Result<Guid>>;

public sealed class CreateRecipeRequestValidator : RecipeRequestValidator<CreateRecipeRequest>
{
}