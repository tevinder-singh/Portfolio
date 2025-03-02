using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace FlavourVault.Recipes.UseCases.CreateRecipe;

internal sealed class CreateRecipeEndPoint : IMinimalApiEndpoint
{
    [ExcludeFromCodeCoverage]
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("recipes", Handle)            
            .WithRequestValidation<CreateRecipeRequest>()
            .Produces<Guid>((int)HttpStatusCode.Created)
            .WithDescription("Create Recipe")
            .WithSummary("Create Recipe")
            .WithTags("Recipies")
            .AllowAnonymous();
    }

    public static async Task<IResult> Handle(CreateRecipeRequest request, IMediator mediator, CancellationToken cancellationToken)
    {
        Result<Guid> result = await mediator.Send(request, cancellationToken);
        return result.ToApiResult();
    }
}