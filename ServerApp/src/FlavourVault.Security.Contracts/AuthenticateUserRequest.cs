namespace FlavourVault.Security.Contracts;

public record AuthenticateUserRequest(string Email, string Password ): IRequest<Result<AuthenticateUserResponse>>;

public class AuthenticateUserRequestValidator : AbstractValidator<AuthenticateUserRequest>
{
    public AuthenticateUserRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();        
    }
}