using AutoMapper;
using FlavourVault.SharedCore.Interfaces;
using FlavourVault.SharedCore.RequestValidations;
using FlavourVault.SharedCore.Results;
using FlavourVault.Security.Contracts;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace FlavourVault.Security.UseCases.AuthenticateUser;

internal sealed class AuthenticateUserEndPoint : IMinimalApiEndpoint
{
    [ExcludeFromCodeCoverage]
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/login", Login)
            .WithSummary("Login user and issue access token")
            .WithRequestValidation<AuthenticateUserRequest>()
            .Produces<AuthenticateUserResponse>()
            .AllowAnonymous();
    }

    public static async Task<IResult> Login(AuthenticateUserRequest request, IMediator mediator, IMapper mapper, CancellationToken cancellationToken)
    {
        var command = mapper.Map<AuthenticateUserCommand>(request);
        Result<AuthenticateUserResponse> result = await mediator.Send(command, cancellationToken);
        return result.ToApiResult();
    }
}