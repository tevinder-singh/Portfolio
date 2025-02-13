using AutoMapper;
using FlavourVault.Recipes.Contracts;
using FlavourVault.SharedCore.Interfaces;
using FlavourVault.SharedCore.RequestValidations;
using FlavourVault.SharedCore.Results;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace FlavourVault.Recipes.UseCases.CreateRecipe;

internal sealed class CreateRecipeEndPoint : IMinimalApiEndpoint
{
    [ExcludeFromCodeCoverage]
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("recipes", Handle)
            .WithSummary("Add new recipe")
            .WithRequestValidation<CreateRecipeRequest>()
            .Produces<Guid>()
            .AllowAnonymous();
    }

    public static async Task<IResult> Handle(CreateRecipeRequest request, IMediator mediator, IMapper mapper, CancellationToken cancellationToken)
    {
        var command = mapper.Map<CreateRecipeCommand>(request);
        Result<Guid> result = await mediator.Send(command, cancellationToken);
        return result.ToApiResult();
    }
}