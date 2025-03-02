using FlavourVault.Recipes.Domain.Recipe;

namespace FlavourVault.Recipes.UseCases.CreateRecipe;

internal sealed class CreateRecipeCommandHandler(        
        IRecipesRepository recipesRepository,
        IRecipiesUnitOfWork unitOfWork
    ) : IRequestHandler<CreateRecipeRequest, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateRecipeRequest request, CancellationToken cancellationToken)
    {
        var result = Recipe.From(request);
        if (!result.IsSuccess)
            return Result.Invalid<Guid>(result.Errors);

        var recipe = result.Value;
        recipesRepository.AddRecipe(recipe);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Created(recipe.Id);
    }
}
