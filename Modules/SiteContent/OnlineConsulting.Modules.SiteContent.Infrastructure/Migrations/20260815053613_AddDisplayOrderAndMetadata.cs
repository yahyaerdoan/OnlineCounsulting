using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddDisplayOrderAndMetadata : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.AddColumn<int>(
            name: "DisplayOrder",
            schema: "SiteContent",
            table: "Testimonials",
            type: "int",
            nullable: false,
            defaultValue: 0);

        _ = migrationBuilder.AddColumn<string>(
            name: "Metadata",
            schema: "SiteContent",
            table: "Testimonials",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<int>(
            name: "DisplayOrder",
            schema: "SiteContent",
            table: "PageBanners",
            type: "int",
            nullable: false,
            defaultValue: 0);

        _ = migrationBuilder.AddColumn<string>(
            name: "Metadata",
            schema: "SiteContent",
            table: "PageBanners",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<int>(
            name: "DisplayOrder",
            schema: "SiteContent",
            table: "HeroSlides",
            type: "int",
            nullable: false,
            defaultValue: 0);

        _ = migrationBuilder.AddColumn<string>(
            name: "Metadata",
            schema: "SiteContent",
            table: "HeroSlides",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<int>(
            name: "DisplayOrder",
            schema: "SiteContent",
            table: "FooterInfos",
            type: "int",
            nullable: false,
            defaultValue: 0);

        _ = migrationBuilder.AddColumn<string>(
            name: "Metadata",
            schema: "SiteContent",
            table: "FooterInfos",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<int>(
            name: "DisplayOrder",
            schema: "SiteContent",
            table: "FeatureHighlights",
            type: "int",
            nullable: false,
            defaultValue: 0);

        _ = migrationBuilder.AddColumn<string>(
            name: "Metadata",
            schema: "SiteContent",
            table: "FeatureHighlights",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<int>(
            name: "DisplayOrder",
            schema: "SiteContent",
            table: "AboutUss",
            type: "int",
            nullable: false,
            defaultValue: 0);

        _ = migrationBuilder.AddColumn<string>(
            name: "Metadata",
            schema: "SiteContent",
            table: "AboutUss",
            type: "nvarchar(max)",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropColumn(
            name: "DisplayOrder",
            schema: "SiteContent",
            table: "Testimonials");

        _ = migrationBuilder.DropColumn(
            name: "Metadata",
            schema: "SiteContent",
            table: "Testimonials");

        _ = migrationBuilder.DropColumn(
            name: "DisplayOrder",
            schema: "SiteContent",
            table: "PageBanners");

        _ = migrationBuilder.DropColumn(
            name: "Metadata",
            schema: "SiteContent",
            table: "PageBanners");

        _ = migrationBuilder.DropColumn(
            name: "DisplayOrder",
            schema: "SiteContent",
            table: "HeroSlides");

        _ = migrationBuilder.DropColumn(
            name: "Metadata",
            schema: "SiteContent",
            table: "HeroSlides");

        _ = migrationBuilder.DropColumn(
            name: "DisplayOrder",
            schema: "SiteContent",
            table: "FooterInfos");

        _ = migrationBuilder.DropColumn(
            name: "Metadata",
            schema: "SiteContent",
            table: "FooterInfos");

        _ = migrationBuilder.DropColumn(
            name: "DisplayOrder",
            schema: "SiteContent",
            table: "FeatureHighlights");

        _ = migrationBuilder.DropColumn(
            name: "Metadata",
            schema: "SiteContent",
            table: "FeatureHighlights");

        _ = migrationBuilder.DropColumn(
            name: "DisplayOrder",
            schema: "SiteContent",
            table: "AboutUss");

        _ = migrationBuilder.DropColumn(
            name: "Metadata",
            schema: "SiteContent",
            table: "AboutUss");
    }
}
