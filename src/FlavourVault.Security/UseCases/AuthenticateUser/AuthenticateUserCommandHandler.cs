using FlavourVault.SharedCore.Data;
using FlavourVault.SharedCore.Results;
using FlavourVault.Security.Contracts;
using FlavourVault.Security.Data.Repositories;
using MediatR;

namespace FlavourVault.Security.UseCases.AuthenticateUser;

internal sealed class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, Result<AuthenticateUserResponse>>
{
    private readonly ILogger<AuthenticateUserCommandHandler> _logger;
    private readonly IUsersRepository usersRepository;

    public AuthenticateUserCommandHandler(ILogger<AuthenticateUserCommandHandler> logger, IUsersRepository usersRepository)
    {
        _logger = logger;
        this.usersRepository = usersRepository;
    }

    public async Task<Result<AuthenticateUserResponse>> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
    {
        //var user = usersRepository.GetByIdAsync(request.Email);

        //user.AddRole(request.RoleId, usersRepository);

        //UnitOfWork.SaveChanges();

        _logger.LogInformation("Authenticating user");
        //var validationResult = await _validator.ValidateAsync(request);
        //if (!validationResult.IsValid)
        //    return Result<AuthenticateUserResponse>.Invalid(validationResult.ToDictionary());

        return Result<AuthenticateUserResponse>.Unauthorized();
        //var user = await _userRepository.GetUserWithCartByEmailAsync(request.EmailAddress);

        //if (user is null)
        //{
        //    return Result<AuthenticateUserResponse>.Unauthorized();
        //}

        //return new UserDTO(Guid.Parse(user!.Id), user.Email!);
    }
}
