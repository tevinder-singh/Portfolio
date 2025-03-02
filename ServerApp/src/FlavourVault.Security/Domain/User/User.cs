using FlavourVault.SharedCore.Domain.Common;
using FlavourVault.Security.Domain.User.Enums;
using FlavourVault.SharedCore.Domain.DomainEvents;

namespace FlavourVault.Security.Domain.User;

internal sealed class User : AggregateRoot, IHasDomainEvent, IAuditableEntity
{
    public User() {}
    public User(string firstName, string lastName, string username, string email)
    {
        FirstName = firstName;
        LastName = lastName;
        FullName = firstName + " " + lastName;
        Username = username;
        Email = email;
        EmailVerified = false;
        Status = UserStatus.Enabled;
    }

    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string FullName { get; private set; } = null!;
    public string Username { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public bool EmailVerified { get; private set; }
    public UserStatus Status { get; private set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? ModifiedBy { get; set; }

    public IList<DomainEvent> DomainEvents => throw new NotImplementedException();
}