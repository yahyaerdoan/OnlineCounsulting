using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Commerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantIdToCommerce : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "Commerce",
                table: "UserAddresses",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000001")); // TenantDefaults.DefaultTenantId - existing rows must backfill to the same tenant the query filter expects

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "Commerce",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000001")); // TenantDefaults.DefaultTenantId - existing rows must backfill to the same tenant the query filter expects

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "Commerce",
                table: "OrderItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000001")); // TenantDefaults.DefaultTenantId - existing rows must backfill to the same tenant the query filter expects

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "Commerce",
                table: "Baskets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000001")); // TenantDefaults.DefaultTenantId - existing rows must backfill to the same tenant the query filter expects

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "Commerce",
                table: "BasketItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000001")); // TenantDefaults.DefaultTenantId - existing rows must backfill to the same tenant the query filter expects
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Commerce",
                table: "UserAddresses");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Commerce",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Commerce",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Commerce",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Commerce",
                table: "BasketItems");
        }
    }
}
