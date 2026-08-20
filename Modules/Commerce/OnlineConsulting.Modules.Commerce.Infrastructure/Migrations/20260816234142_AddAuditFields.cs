using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Commerce.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddAuditFields : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "Commerce",
            table: "UserAddresses",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            schema: "Commerce",
            table: "UserAddresses",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            schema: "Commerce",
            table: "UserAddresses",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "Commerce",
            table: "Orders",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            schema: "Commerce",
            table: "Orders",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            schema: "Commerce",
            table: "Orders",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "Commerce",
            table: "OrderItems",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            schema: "Commerce",
            table: "OrderItems",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            schema: "Commerce",
            table: "OrderItems",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "Commerce",
            table: "Baskets",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            schema: "Commerce",
            table: "Baskets",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            schema: "Commerce",
            table: "Baskets",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "Commerce",
            table: "BasketItems",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            schema: "Commerce",
            table: "BasketItems",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            schema: "Commerce",
            table: "BasketItems",
            type: "nvarchar(max)",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropColumn(
            name: "CreatedBy",
            schema: "Commerce",
            table: "UserAddresses");

        _ = migrationBuilder.DropColumn(
            name: "DeletedBy",
            schema: "Commerce",
            table: "UserAddresses");

        _ = migrationBuilder.DropColumn(
            name: "UpdatedBy",
            schema: "Commerce",
            table: "UserAddresses");

        _ = migrationBuilder.DropColumn(
            name: "CreatedBy",
            schema: "Commerce",
            table: "Orders");

        _ = migrationBuilder.DropColumn(
            name: "DeletedBy",
            schema: "Commerce",
            table: "Orders");

        _ = migrationBuilder.DropColumn(
            name: "UpdatedBy",
            schema: "Commerce",
            table: "Orders");

        _ = migrationBuilder.DropColumn(
            name: "CreatedBy",
            schema: "Commerce",
            table: "OrderItems");

        _ = migrationBuilder.DropColumn(
            name: "DeletedBy",
            schema: "Commerce",
            table: "OrderItems");

        _ = migrationBuilder.DropColumn(
            name: "UpdatedBy",
            schema: "Commerce",
            table: "OrderItems");

        _ = migrationBuilder.DropColumn(
            name: "CreatedBy",
            schema: "Commerce",
            table: "Baskets");

        _ = migrationBuilder.DropColumn(
            name: "DeletedBy",
            schema: "Commerce",
            table: "Baskets");

        _ = migrationBuilder.DropColumn(
            name: "UpdatedBy",
            schema: "Commerce",
            table: "Baskets");

        _ = migrationBuilder.DropColumn(
            name: "CreatedBy",
            schema: "Commerce",
            table: "BasketItems");

        _ = migrationBuilder.DropColumn(
            name: "DeletedBy",
            schema: "Commerce",
            table: "BasketItems");

        _ = migrationBuilder.DropColumn(
            name: "UpdatedBy",
            schema: "Commerce",
            table: "BasketItems");
    }
}
