using FlavourVault.SharedCore.Data;
using FlavourVault.Security.Domain.IdentityProvider;
using FlavourVault.Security.Domain.Permission;
using FlavourVault.Security.Domain.Role;
using FlavourVault.Security.Domain.Role.Entities;
using FlavourVault.Security.Domain.User;
using FlavourVault.Security.Domain.User.Entities;
using Microsoft.EntityFrameworkCore;
using FlavourVault.SharedCore.Interfaces;

namespace FlavourVault.Security.Data;

internal sealed class SecurityDbContext : DbContextBase
{
    public SecurityDbContext(DbContextOptions<SecurityDbContext> dbContextOptions, IUserContext userContext) : base(dbContextOptions,userContext, SchemaName)
    {            
    }

    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<IdentityProvider> IdentityProviders { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }    
    public DbSet<UserPassword> UserPasswords { get; set; }
    public DbSet<UserPreviousPassword> UserPreviousPasswords { get; set; }
    public DbSet<UserFederatedIdentity> UserFederatedIdentities { get; set; }
    public DbSet<UserLoginFailure> UserLoginFailures { get; set; }
    public DbSet<UserMultiFactorAuthentication> UserMultiFactorAuthentications { get; set; }
    public DbSet<UserRequiredAction> UserRequiredActions { get; set; }
    public DbSet<UserToken> UserTokens { get; set; }
    public DbSet<UserDevice> UserDevices { get; set; }       

    public const string SchemaName = "Security";
}