using FluentValidation;

namespace FlavourVault.Security.Contracts;

public record AuthenticateUserRequest(string Email, string Password );

public class AuthenticateUserRequestValidator : AbstractValidator<AuthenticateUserRequest>
{
    public AuthenticateUserRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();        
    }
}