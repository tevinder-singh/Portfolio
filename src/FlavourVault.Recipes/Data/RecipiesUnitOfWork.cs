using FlavourVault.SharedCore.Data;
using Microsoft.EntityFrameworkCore;

namespace FlavourVault.Recipes.Data;

internal sealed class RecipiesUnitOfWork : UnitOfWork, IRecipiesUnitOfWork
{
    public RecipiesUnitOfWork(RecipesDbContext dbContext) : base(dbContext)
    {
    }
}
