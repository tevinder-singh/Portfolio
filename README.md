Loosely coupled modular monolith application
Communication done through MediatR
Outbox Pattern is used for sending messages and events

# flavourvault

dotnet ef --project FlavourVault.Recipes --startup-project FlavourVault.Web migrations add Initial --context RecipesDbContext
dotnet ef --project FlavourVault.Recipes --startup-project FlavourVault.Web database update --context RecipesDbContext

dotnet ef --project FlavourVault.Security --startup-project FlavourVault.Web migrations add Initial --context SecurityDbContext
dotnet ef --project FlavourVault.Security --startup-project FlavourVault.Web database update --context SecurityDbContext

dotnet ef --project FlavourVault.Audit --startup-project FlavourVault.Web migrations add Initial --context AuditDbContext
dotnet ef --project FlavourVault.Audit --startup-project FlavourVault.Web database update --context AuditDbContext

# Store Connections strings in user secrets for local debugging
 dotnet user-secrets set "ConnectionStrings:DbConnectionString" "CONNECTION_STRING"

# Add .env file in docker compose to store
DB_PASSWORD=
DB_ConnectionString=