using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Identity.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddAuditFields : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "Identity",
            table: "RefreshTokens",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            schema: "Identity",
            table: "RefreshTokens",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            schema: "Identity",
            table: "RefreshTokens",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AlterColumn<DateTime>(
            name: "CreatedDate",
            schema: "Identity",
            table: "AspNetUsers",
            type: "datetime2",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
            oldClrType: typeof(DateTime),
            oldType: "datetime2",
            oldNullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropColumn(
            name: "CreatedBy",
            schema: "Identity",
            table: "RefreshTokens");

        _ = migrationBuilder.DropColumn(
            name: "DeletedBy",
            schema: "Identity",
            table: "RefreshTokens");

        _ = migrationBuilder.DropColumn(
            name: "UpdatedBy",
            schema: "Identity",
            table: "RefreshTokens");

        _ = migrationBuilder.AlterColumn<DateTime>(
            name: "CreatedDate",
            schema: "Identity",
            table: "AspNetUsers",
            type: "datetime2",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "datetime2");
    }
}
