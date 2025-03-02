using System.Diagnostics.CodeAnalysis;

namespace FlavourVault.Security.UseCases.AuthenticateUser;

internal sealed class AuthenticateUserEndPoint : IMinimalApiEndpoint
{
    [ExcludeFromCodeCoverage]
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/login", Handle)
            .WithSummary("Login user and issue access token")
            .WithRequestValidation<AuthenticateUserRequest>()
            .Produces<AuthenticateUserResponse>()
            .AllowAnonymous();
    }

    public static async Task<IResult> Handle(AuthenticateUserRequest request, IMediator mediator, CancellationToken cancellationToken)
    {
        Result<AuthenticateUserResponse> result = await mediator.Send(request, cancellationToken);
        return result.ToApiResult();
    }
}