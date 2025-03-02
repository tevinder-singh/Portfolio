using FlavourVault.SharedCore.Domain.Common;

namespace FlavourVault.Recipes.Domain.Recipe;

internal sealed class Recipe : AggregateRoot, IAuditableEntity
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? ModifiedBy { get; set; }

    public Result UpdateFrom(UpdateRecipeRequest updateRecipeCommand)
    {
        Name = updateRecipeCommand.Name;
        Description = updateRecipeCommand.Description;
        return Result.Success();
    }

    public static Result<Recipe> From(CreateRecipeRequest command)
    {
        var recipe = new Recipe
        {
            Name = command.Name,
            Description = command.Description,
        };
        return Result.Success(recipe);
    }
}