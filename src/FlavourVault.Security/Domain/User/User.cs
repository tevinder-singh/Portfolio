using FlavourVault.SharedCore.Domain.Common;
using FlavourVault.Security.Domain.User.Enums;

namespace FlavourVault.Security.Domain.User;

internal sealed class User : AggregateRoot
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

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string FullName { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }
    public bool EmailVerified { get; private set; }
    public UserStatus Status { get; private set; }    
}