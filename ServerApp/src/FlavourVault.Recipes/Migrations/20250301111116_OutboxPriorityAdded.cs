using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlavourVault.Recipes.Migrations
{
    /// <inheritdoc />
    public partial class OutboxPriorityAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Priority",
                schema: "Recipes",
                table: "OutboxMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                schema: "Recipes",
                table: "OutboxMessages");
        }
    }
}
