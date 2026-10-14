using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPageHeroImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AboutHeroImageUrl",
                schema: "SiteContent",
                table: "LandingPages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactHeroImageUrl",
                schema: "SiteContent",
                table: "LandingPages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeaturesHeroImageUrl",
                schema: "SiteContent",
                table: "LandingPages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IndustriesHeroImageUrl",
                schema: "SiteContent",
                table: "LandingPages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AboutHeroImageUrl",
                schema: "SiteContent",
                table: "LandingPages");

            migrationBuilder.DropColumn(
                name: "ContactHeroImageUrl",
                schema: "SiteContent",
                table: "LandingPages");

            migrationBuilder.DropColumn(
                name: "FeaturesHeroImageUrl",
                schema: "SiteContent",
                table: "LandingPages");

            migrationBuilder.DropColumn(
                name: "IndustriesHeroImageUrl",
                schema: "SiteContent",
                table: "LandingPages");
        }
    }
}
