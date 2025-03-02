namespace FlavourVault.Security.UseCases.AuthenticateUser;

internal sealed class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserRequest, Result<AuthenticateUserResponse>>
{
    private readonly ILogger<AuthenticateUserCommandHandler> _logger;
    private readonly IUsersRepository usersRepository;

    public AuthenticateUserCommandHandler(ILogger<AuthenticateUserCommandHandler> logger, IUsersRepository usersRepository)
    {
        _logger = logger;
        this.usersRepository = usersRepository;
    }

    public async Task<Result<AuthenticateUserResponse>> Handle(AuthenticateUserRequest request, CancellationToken cancellationToken)
    {
        //var user = usersRepository.GetByIdAsync(request.Email);

        //user.AddRole(request.RoleId, usersRepository);

        //UnitOfWork.SaveChanges();

        _logger.LogInformation("Authenticating user");
        //var validationResult = await _validator.ValidateAsync(request);
        //if (!validationResult.IsValid)
        //    return Result<AuthenticateUserResponse>.Invalid(validationResult.ToDictionary());

        return Result.Unauthorized<AuthenticateUserResponse>();
        //var user = await _userRepository.GetUserWithCartByEmailAsync(request.EmailAddress);

        //if (user is null)
        //{
        //    return Result<AuthenticateUserResponse>.Unauthorized();
        //}

        //return new UserDTO(Guid.Parse(user!.Id), user.Email!);
    }
}
