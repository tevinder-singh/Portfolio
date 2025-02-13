using FlavourVault.Recipes.Domain.Recipe;
using FlavourVault.SharedCore.Data;
using FlavourVault.SharedCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace FlavourVault.Recipes.Data;

internal sealed class RecipesDbContext: DbContextBase
{
    public RecipesDbContext(DbContextOptions<RecipesDbContext> dbContextOptions, IUserContext userContext) : base(dbContextOptions, userContext, SchemaName)
    {            
    }


    public DbSet<Recipe> Recipes { get; set; }

    public const string SchemaName = "Recipes";
}