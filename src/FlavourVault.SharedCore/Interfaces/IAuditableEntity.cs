namespace FlavourVault.SharedCore.Interfaces;
public interface IAuditableEntity
{
    Guid Id { get; }
    DateTime CreatedOn { get; set; }
    DateTime? ModifiedOn { get; set; }
    Guid? CreatedBy { get; set; }
    Guid? ModifiedBy { get; set; }
}