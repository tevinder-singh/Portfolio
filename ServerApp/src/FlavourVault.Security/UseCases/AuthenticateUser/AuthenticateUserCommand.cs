using AutoMapper;
using FlavourVault.SharedCore.Results;
using FlavourVault.Security.Contracts;
using FluentValidation;
using MediatR;

namespace FlavourVault.Security.UseCases.AuthenticateUser;

internal record AuthenticateUserCommand (string Email, string Password) : IRequest<Result<AuthenticateUserResponse>>;

internal sealed class AuthenticateUserCommandValidator : AbstractValidator<AuthenticateUserCommand>
{
    public AuthenticateUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

internal sealed class AuthenticateUserRequestProfile: Profile
{
    public AuthenticateUserRequestProfile()
    {
        CreateMap<AuthenticateUserRequest, AuthenticateUserCommand>();
    }
}
