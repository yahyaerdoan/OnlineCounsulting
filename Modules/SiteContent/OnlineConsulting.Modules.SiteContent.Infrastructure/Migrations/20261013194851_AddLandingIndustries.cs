using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLandingIndustries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IndustriesEyebrow",
                schema: "SiteContent",
                table: "LandingPages",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IndustriesLead",
                schema: "SiteContent",
                table: "LandingPages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IndustriesTitle",
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
                name: "IndustriesEyebrow",
                schema: "SiteContent",
                table: "LandingPages");

            migrationBuilder.DropColumn(
                name: "IndustriesLead",
                schema: "SiteContent",
                table: "LandingPages");

            migrationBuilder.DropColumn(
                name: "IndustriesTitle",
                schema: "SiteContent",
                table: "LandingPages");
        }
    }
}
