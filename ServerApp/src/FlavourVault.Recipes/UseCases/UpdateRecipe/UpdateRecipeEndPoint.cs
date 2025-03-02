using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace FlavourVault.Recipes.UseCases.UpdateRecipe;

internal sealed class UpdateRecipeEndPoint : IMinimalApiEndpoint
{
    [ExcludeFromCodeCoverage]
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("recipes/{id}", Handle)            
            .WithRequestValidation<UpdateRecipeRequest>()
            .Produces((int)HttpStatusCode.OK)
            .WithDescription("Update Recipe")
            .WithSummary("Update Recipe")
            .WithTags("Recipies")
            .AllowAnonymous();
    }

    public static async Task<IResult> Handle(Guid id, UpdateRecipeRequest request, IMediator mediator, CancellationToken cancellationToken)
    {
        request.Id = id;
        Result result = await mediator.Send(request, cancellationToken);
        return result.ToApiResult();
    }
}