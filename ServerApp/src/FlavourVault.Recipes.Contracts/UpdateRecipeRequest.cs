using System.Text.Json.Serialization;

namespace FlavourVault.Recipes.Contracts;

public sealed record UpdateRecipeRequest(string Name, string Description) : RecipeRequest(Name, Description), IRequest<Result>
{
    [JsonIgnore]
    public Guid? Id { get; set; }
}

public sealed class UpdateRecipeRequestValidator : RecipeRequestValidator<UpdateRecipeRequest>
{
}