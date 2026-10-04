using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Commerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BasketItemsBelongToBasket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_BasketItems_Baskets_BasketId",
                schema: "Commerce",
                table: "BasketItems",
                column: "BasketId",
                principalSchema: "Commerce",
                principalTable: "Baskets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasketItems_Baskets_BasketId",
                schema: "Commerce",
                table: "BasketItems");
        }
    }
}
