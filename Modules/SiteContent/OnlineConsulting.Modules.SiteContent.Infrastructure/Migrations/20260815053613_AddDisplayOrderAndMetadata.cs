using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddDisplayOrderAndMetadata : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "DisplayOrder",
            schema: "SiteContent",
            table: "Testimonials",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "Metadata",
            schema: "SiteContent",
            table: "Testimonials",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "DisplayOrder",
            schema: "SiteContent",
            table: "PageBanners",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "Metadata",
            schema: "SiteContent",
            table: "PageBanners",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "DisplayOrder",
            schema: "SiteContent",
            table: "HeroSlides",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "Metadata",
            schema: "SiteContent",
            table: "HeroSlides",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "DisplayOrder",
            schema: "SiteContent",
            table: "FooterInfos",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "Metadata",
            schema: "SiteContent",
            table: "FooterInfos",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "DisplayOrder",
            schema: "SiteContent",
            table: "FeatureHighlights",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "Metadata",
            schema: "SiteContent",
            table: "FeatureHighlights",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "DisplayOrder",
            schema: "SiteContent",
            table: "AboutUss",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "Metadata",
            schema: "SiteContent",
            table: "AboutUss",
            type: "nvarchar(max)",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "DisplayOrder",
            schema: "SiteContent",
            table: "Testimonials");

        migrationBuilder.DropColumn(
            name: "Metadata",
            schema: "SiteContent",
            table: "Testimonials");

        migrationBuilder.DropColumn(
            name: "DisplayOrder",
            schema: "SiteContent",
            table: "PageBanners");

        migrationBuilder.DropColumn(
            name: "Metadata",
            schema: "SiteContent",
            table: "PageBanners");

        migrationBuilder.DropColumn(
            name: "DisplayOrder",
            schema: "SiteContent",
            table: "HeroSlides");

        migrationBuilder.DropColumn(
            name: "Metadata",
            schema: "SiteContent",
            table: "HeroSlides");

        migrationBuilder.DropColumn(
            name: "DisplayOrder",
            schema: "SiteContent",
            table: "FooterInfos");

        migrationBuilder.DropColumn(
            name: "Metadata",
            schema: "SiteContent",
            table: "FooterInfos");

        migrationBuilder.DropColumn(
            name: "DisplayOrder",
            schema: "SiteContent",
            table: "FeatureHighlights");

        migrationBuilder.DropColumn(
            name: "Metadata",
            schema: "SiteContent",
            table: "FeatureHighlights");

        migrationBuilder.DropColumn(
            name: "DisplayOrder",
            schema: "SiteContent",
            table: "AboutUss");

        migrationBuilder.DropColumn(
            name: "Metadata",
            schema: "SiteContent",
            table: "AboutUss");
    }
}
