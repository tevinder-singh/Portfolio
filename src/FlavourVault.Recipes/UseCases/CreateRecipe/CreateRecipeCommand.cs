using AutoMapper;
using FlavourVault.Recipes.Contracts;
using FlavourVault.SharedCore.Results;
using FluentValidation;
using MediatR;

namespace FlavourVault.Recipes.UseCases.CreateRecipe;

internal record CreateRecipeCommand (string Name, string Description) : IRequest<Result<Guid>>;

internal sealed class CreateRecipeCommandValidator : AbstractValidator<CreateRecipeCommand>
{
    public CreateRecipeCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
    }
}

internal sealed class CreateRecipeRequestProfile : Profile
{
    public CreateRecipeRequestProfile()
    {
        CreateMap<CreateRecipeRequest, CreateRecipeCommand>();
    }
}
