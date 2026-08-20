using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.FeatureFlags.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddAuditFields : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "FeatureFlags",
            table: "FeatureFlags",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            schema: "FeatureFlags",
            table: "FeatureFlags",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            schema: "FeatureFlags",
            table: "FeatureFlags",
            type: "nvarchar(max)",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropColumn(
            name: "CreatedBy",
            schema: "FeatureFlags",
            table: "FeatureFlags");

        _ = migrationBuilder.DropColumn(
            name: "DeletedBy",
            schema: "FeatureFlags",
            table: "FeatureFlags");

        _ = migrationBuilder.DropColumn(
            name: "UpdatedBy",
            schema: "FeatureFlags",
            table: "FeatureFlags");
    }
}
