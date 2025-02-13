namespace FlavourVault.Security.Domain.User.Enums;

internal enum RequiredAction
{
    None = 0,
    VerifyEmail = 1,
    ChangePassword = 2,
    ConfigureMFA = 3
}
