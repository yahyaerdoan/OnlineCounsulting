using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Categories.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddAuditFields : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "Categories",
            table: "Categories",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            schema: "Categories",
            table: "Categories",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            schema: "Categories",
            table: "Categories",
            type: "nvarchar(max)",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropColumn(
            name: "CreatedBy",
            schema: "Categories",
            table: "Categories");

        _ = migrationBuilder.DropColumn(
            name: "DeletedBy",
            schema: "Categories",
            table: "Categories");

        _ = migrationBuilder.DropColumn(
            name: "UpdatedBy",
            schema: "Categories",
            table: "Categories");
    }
}
