using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAboutHero : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AboutHeroEyebrow",
                schema: "SiteContent",
                table: "LandingPages",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AboutHeroLead",
                schema: "SiteContent",
                table: "LandingPages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AboutHeroTitle",
                schema: "SiteContent",
                table: "LandingPages",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AboutHeroEyebrow",
                schema: "SiteContent",
                table: "LandingPages");

            migrationBuilder.DropColumn(
                name: "AboutHeroLead",
                schema: "SiteContent",
                table: "LandingPages");

            migrationBuilder.DropColumn(
                name: "AboutHeroTitle",
                schema: "SiteContent",
                table: "LandingPages");
        }
    }
}
