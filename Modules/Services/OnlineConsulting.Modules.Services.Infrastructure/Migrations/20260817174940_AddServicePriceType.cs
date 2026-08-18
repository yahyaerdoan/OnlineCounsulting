using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Services.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddServicePriceType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PriceMax",
                schema: "Services",
                table: "Services",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PriceType",
                schema: "Services",
                table: "Services",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceMax",
                schema: "Services",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "PriceType",
                schema: "Services",
                table: "Services");
        }
    }
}
