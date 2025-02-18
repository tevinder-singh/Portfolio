using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlavourVault.Recipes.Migrations
{
    /// <inheritdoc />
    public partial class OutboxUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                schema: "Recipes",
                table: "OutboxMessages",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RetryCount",
                schema: "Recipes",
                table: "OutboxMessages");
        }
    }
}
