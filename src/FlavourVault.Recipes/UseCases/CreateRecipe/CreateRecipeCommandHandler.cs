using FlavourVault.Recipes.Data;
using FlavourVault.Recipes.Data.Repositories;
using FlavourVault.Recipes.Domain.Recipe;
using FlavourVault.SharedCore.Results;
using MediatR;

namespace FlavourVault.Recipes.UseCases.CreateRecipe;

internal sealed class CreateRecipeCommandHandler(
        ILogger<CreateRecipeCommandHandler> logger,
        IRecipesRepository recipesRepository,
        IRecipiesUnitOfWork unitOfWork
    ) : IRequestHandler<CreateRecipeCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateRecipeCommand request, CancellationToken cancellationToken)
    {
        var recipe = new Recipe
        {
            Name = request.Name,
            Description = request.Description,
        };

        recipesRepository.AddRecipe(recipe);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Created(recipe.Id);
    }
}
