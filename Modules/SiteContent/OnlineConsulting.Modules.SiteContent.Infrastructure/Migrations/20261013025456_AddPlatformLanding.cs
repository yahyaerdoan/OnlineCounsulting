using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlatformLanding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LandingBlocks",
                schema: "SiteContent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Section = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandingBlocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LandingPages",
                schema: "SiteContent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HeroEyebrow = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    HeroTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    HeroLead = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PrimaryCtaText = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    SecondaryCtaText = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    FeaturesEyebrow = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    FeaturesTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FeaturesLead = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StepsEyebrow = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    StepsTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StepsLead = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PricingEyebrow = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    PricingTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PricingLead = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FaqEyebrow = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    FaqTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ClosingTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ClosingLead = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandingPages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LandingBlocks_TenantId_DeletedDate",
                schema: "SiteContent",
                table: "LandingBlocks",
                columns: new[] { "TenantId", "DeletedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_LandingPages_TenantId_DeletedDate",
                schema: "SiteContent",
                table: "LandingPages",
                columns: new[] { "TenantId", "DeletedDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LandingBlocks",
                schema: "SiteContent");

            migrationBuilder.DropTable(
                name: "LandingPages",
                schema: "SiteContent");
        }
    }
}
