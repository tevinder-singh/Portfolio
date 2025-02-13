using FluentValidation;

namespace FlavourVault.Recipes.Contracts;

public record CreateRecipeRequest(string Name, string Description);

public class CreateRecipeRequestValidator : AbstractValidator<CreateRecipeRequest>
{
    public CreateRecipeRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
    }
}