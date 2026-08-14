using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Commerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGuestBasketSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "Commerce",
                table: "Baskets",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "GuestId",
                schema: "Commerce",
                table: "Baskets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Baskets_GuestId",
                schema: "Commerce",
                table: "Baskets",
                column: "GuestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Baskets_GuestId",
                schema: "Commerce",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "GuestId",
                schema: "Commerce",
                table: "Baskets");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "Commerce",
                table: "Baskets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
