namespace FlavourVault.Recipes.Contracts;
public abstract record RecipeRequest (string Name, string Description);

public abstract class RecipeRequestValidator<T> : AbstractValidator<T> where T : RecipeRequest
{
    protected RecipeRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
    }
}