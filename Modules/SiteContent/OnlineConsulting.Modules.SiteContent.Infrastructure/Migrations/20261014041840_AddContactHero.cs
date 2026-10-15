using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContactHero : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactHeroEyebrow",
                schema: "SiteContent",
                table: "LandingPages",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactHeroLead",
                schema: "SiteContent",
                table: "LandingPages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactHeroTitle",
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
                name: "ContactHeroEyebrow",
                schema: "SiteContent",
                table: "LandingPages");

            migrationBuilder.DropColumn(
                name: "ContactHeroLead",
                schema: "SiteContent",
                table: "LandingPages");

            migrationBuilder.DropColumn(
                name: "ContactHeroTitle",
                schema: "SiteContent",
                table: "LandingPages");
        }
    }
}
