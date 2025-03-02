namespace FlavourVault.Recipes.UseCases.UpdateRecipe;

internal sealed class UpdateRecipeCommandHandler(        
        IRecipesRepository recipesRepository,
        IRecipiesUnitOfWork unitOfWork
    ) : IRequestHandler<UpdateRecipeRequest, Result>
{
    public async Task<Result> Handle(UpdateRecipeRequest request, CancellationToken cancellationToken)
    {
        var recipe = await recipesRepository.GetRecipeByIdAsync(request.Id.GetValueOrDefault());
        if (recipe == null)
            return Result.NotFound();

        var result = recipe.UpdateFrom(request);
        if(!result.IsSuccess)
            return result;
                
        recipesRepository.UpdateRecipe(recipe);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}