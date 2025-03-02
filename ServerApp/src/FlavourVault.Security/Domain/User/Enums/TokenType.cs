namespace FlavourVault.Security.Domain.User.Enums;

internal enum TokenType
{
    None = 0,
    AccessToken = 1,
    RefreshToken = 2,
    EmailVerification = 3,
    ResetPassword = 4,
    MFA = 5
}
