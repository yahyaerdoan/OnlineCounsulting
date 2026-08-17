using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Commerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Commerce",
                table: "UserAddresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "Commerce",
                table: "UserAddresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "Commerce",
                table: "UserAddresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Commerce",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "Commerce",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "Commerce",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Commerce",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "Commerce",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "Commerce",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Commerce",
                table: "Baskets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "Commerce",
                table: "Baskets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "Commerce",
                table: "Baskets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Commerce",
                table: "BasketItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "Commerce",
                table: "BasketItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "Commerce",
                table: "BasketItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Commerce",
                table: "UserAddresses");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "Commerce",
                table: "UserAddresses");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "Commerce",
                table: "UserAddresses");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Commerce",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "Commerce",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "Commerce",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Commerce",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "Commerce",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "Commerce",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Commerce",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "Commerce",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "Commerce",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Commerce",
                table: "BasketItems");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "Commerce",
                table: "BasketItems");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "Commerce",
                table: "BasketItems");
        }
    }
}
