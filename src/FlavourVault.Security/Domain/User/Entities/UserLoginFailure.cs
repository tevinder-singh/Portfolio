using FlavourVault.SharedCore.Domain.Common;

namespace FlavourVault.Security.Domain.User.Entities;

internal sealed class UserLoginFailure : Entity
{
    public Guid UserId { get; set; }
    public int? FailedPasswordAttemptCount { get; set; }
    public int? FailedPINAttemptCount { get; set; }
    public DateTime? LastFailedPasswordAttemptDate { get; set; }
    public DateTime? LockedOutDate { get; set; }
}