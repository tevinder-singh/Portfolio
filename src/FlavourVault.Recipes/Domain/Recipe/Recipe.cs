using FlavourVault.SharedCore.Domain.Common;
using FlavourVault.SharedCore.Interfaces;

namespace FlavourVault.Recipes.Domain.Recipe;

internal sealed class Recipe : AggregateRoot, IAuditableEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? ModifiedBy { get; set; }
}