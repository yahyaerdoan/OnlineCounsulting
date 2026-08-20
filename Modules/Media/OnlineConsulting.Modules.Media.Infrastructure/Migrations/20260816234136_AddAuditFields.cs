using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Media.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddAuditFields : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "Media",
            table: "MediaAssets",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            schema: "Media",
            table: "MediaAssets",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            schema: "Media",
            table: "MediaAssets",
            type: "nvarchar(max)",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropColumn(
            name: "CreatedBy",
            schema: "Media",
            table: "MediaAssets");

        _ = migrationBuilder.DropColumn(
            name: "DeletedBy",
            schema: "Media",
            table: "MediaAssets");

        _ = migrationBuilder.DropColumn(
            name: "UpdatedBy",
            schema: "Media",
            table: "MediaAssets");
    }
}
