using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlavourVault.Recipes.Migrations
{
    /// <inheritdoc />
    public partial class Migration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                schema: "Recipes",
                table: "Recipes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                schema: "Recipes",
                table: "Recipes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                schema: "Recipes",
                table: "Recipes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                schema: "Recipes",
                table: "Recipes",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Recipes",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                schema: "Recipes",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "Recipes",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                schema: "Recipes",
                table: "Recipes");
        }
    }
}
