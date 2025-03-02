using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlavourVault.Recipes.Migrations
{
    /// <inheritdoc />
    public partial class OutboxRetryDateAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "NextRetryDate",
                schema: "Recipes",
                table: "OutboxMessages",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NextRetryDate",
                schema: "Recipes",
                table: "OutboxMessages");
        }
    }
}
