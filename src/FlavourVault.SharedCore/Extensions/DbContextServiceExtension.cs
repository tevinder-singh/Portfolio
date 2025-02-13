using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlavourVault.SharedCore.Extensions;
public static class DbContextServiceExtension
{
    public static IServiceCollection AddDatabaseContext<T>(this IServiceCollection services, IConfiguration configuration, string schemaName) where T : DbContext
    {
        services.AddDbContext<T>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DbConnectionString"), option =>
            {
                option.MigrationsAssembly(typeof(T).Assembly.FullName);
                option.MigrationsHistoryTable(HistoryRepository.DefaultTableName, schemaName);
                option.TranslateParameterizedCollectionsToConstants();
            });
            options.ConfigureWarnings(w => w.Ignore(RelationalEventId.BoolWithDefaultWarning));
        });
        return services;
    }
}