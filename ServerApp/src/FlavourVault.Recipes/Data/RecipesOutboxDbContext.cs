using FlavourVault.SharedCore.Data;
using Microsoft.EntityFrameworkCore;

namespace FlavourVault.Recipes.Data;
internal sealed class RecipesOutboxDbContext(DbContextOptions<RecipesOutboxDbContext> dbContextOptions) : OutboxDbContextBase(dbContextOptions, RecipesDbContext.SchemaName)
{
}
