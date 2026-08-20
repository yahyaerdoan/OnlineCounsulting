using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddAuditFields : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "SiteContent",
            table: "Testimonials",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            schema: "SiteContent",
            table: "Testimonials",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            schema: "SiteContent",
            table: "Testimonials",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "SiteContent",
            table: "SocialLinks",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            schema: "SiteContent",
            table: "SocialLinks",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            schema: "SiteContent",
            table: "SocialLinks",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "SiteContent",
            table: "ServiceProcessSteps",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            schema: "SiteContent",
            table: "ServiceProcessSteps",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            schema: "SiteContent",
            table: "ServiceProcessSteps",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "SiteContent",
            table: "ServiceOfferings",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            schema: "SiteContent",
            table: "ServiceOfferings",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            schema: "SiteContent",
            table: "ServiceOfferings",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "SiteContent",
            table: "PartnershipSocialLinks",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            schema: "SiteContent",
            table: "PartnershipSocialLinks",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            schema: "SiteContent",
            table: "PartnershipSocialLinks",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "SiteContent",
            table: "Partnerships",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            schema: "SiteContent",
            table: "Partnerships",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            schema: "SiteContent",
            table: "Partnerships",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "SiteContent",
            table: "PageBanners",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            schema: "SiteContent",
            table: "PageBanners",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            schema: "SiteContent",
            table: "PageBanners",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "SiteContent",
            table: "HeroSlides",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            schema: "SiteContent",
            table: "HeroSlides",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            schema: "SiteContent",
            table: "HeroSlides",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "SiteContent",
            table: "GalleryItems",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            schema: "SiteContent",
            table: "GalleryItems",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            schema: "SiteContent",
            table: "GalleryItems",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "SiteContent",
            table: "GalleryItemCategories",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            schema: "SiteContent",
            table: "GalleryItemCategories",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            schema: "SiteContent",
            table: "GalleryItemCategories",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "SiteContent",
            table: "GalleryCategories",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            schema: "SiteContent",
            table: "GalleryCategories",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            schema: "SiteContent",
            table: "GalleryCategories",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "SiteContent",
            table: "FooterInfos",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            schema: "SiteContent",
            table: "FooterInfos",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            schema: "SiteContent",
            table: "FooterInfos",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "SiteContent",
            table: "FeatureHighlights",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            schema: "SiteContent",
            table: "FeatureHighlights",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            schema: "SiteContent",
            table: "FeatureHighlights",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "SiteContent",
            table: "AboutUss",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            schema: "SiteContent",
            table: "AboutUss",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            schema: "SiteContent",
            table: "AboutUss",
            type: "nvarchar(max)",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropColumn(
            name: "CreatedBy",
            schema: "SiteContent",
            table: "Testimonials");

        _ = migrationBuilder.DropColumn(
            name: "DeletedBy",
            schema: "SiteContent",
            table: "Testimonials");

        _ = migrationBuilder.DropColumn(
            name: "UpdatedBy",
            schema: "SiteContent",
            table: "Testimonials");

        _ = migrationBuilder.DropColumn(
            name: "CreatedBy",
            schema: "SiteContent",
            table: "SocialLinks");

        _ = migrationBuilder.DropColumn(
            name: "DeletedBy",
            schema: "SiteContent",
            table: "SocialLinks");

        _ = migrationBuilder.DropColumn(
            name: "UpdatedBy",
            schema: "SiteContent",
            table: "SocialLinks");

        _ = migrationBuilder.DropColumn(
            name: "CreatedBy",
            schema: "SiteContent",
            table: "ServiceProcessSteps");

        _ = migrationBuilder.DropColumn(
            name: "DeletedBy",
            schema: "SiteContent",
            table: "ServiceProcessSteps");

        _ = migrationBuilder.DropColumn(
            name: "UpdatedBy",
            schema: "SiteContent",
            table: "ServiceProcessSteps");

        _ = migrationBuilder.DropColumn(
            name: "CreatedBy",
            schema: "SiteContent",
            table: "ServiceOfferings");

        _ = migrationBuilder.DropColumn(
            name: "DeletedBy",
            schema: "SiteContent",
            table: "ServiceOfferings");

        _ = migrationBuilder.DropColumn(
            name: "UpdatedBy",
            schema: "SiteContent",
            table: "ServiceOfferings");

        _ = migrationBuilder.DropColumn(
            name: "CreatedBy",
            schema: "SiteContent",
            table: "PartnershipSocialLinks");

        _ = migrationBuilder.DropColumn(
            name: "DeletedBy",
            schema: "SiteContent",
            table: "PartnershipSocialLinks");

        _ = migrationBuilder.DropColumn(
            name: "UpdatedBy",
            schema: "SiteContent",
            table: "PartnershipSocialLinks");

        _ = migrationBuilder.DropColumn(
            name: "CreatedBy",
            schema: "SiteContent",
            table: "Partnerships");

        _ = migrationBuilder.DropColumn(
            name: "DeletedBy",
            schema: "SiteContent",
            table: "Partnerships");

        _ = migrationBuilder.DropColumn(
            name: "UpdatedBy",
            schema: "SiteContent",
            table: "Partnerships");

        _ = migrationBuilder.DropColumn(
            name: "CreatedBy",
            schema: "SiteContent",
            table: "PageBanners");

        _ = migrationBuilder.DropColumn(
            name: "DeletedBy",
            schema: "SiteContent",
            table: "PageBanners");

        _ = migrationBuilder.DropColumn(
            name: "UpdatedBy",
            schema: "SiteContent",
            table: "PageBanners");

        _ = migrationBuilder.DropColumn(
            name: "CreatedBy",
            schema: "SiteContent",
            table: "HeroSlides");

        _ = migrationBuilder.DropColumn(
            name: "DeletedBy",
            schema: "SiteContent",
            table: "HeroSlides");

        _ = migrationBuilder.DropColumn(
            name: "UpdatedBy",
            schema: "SiteContent",
            table: "HeroSlides");

        _ = migrationBuilder.DropColumn(
            name: "CreatedBy",
            schema: "SiteContent",
            table: "GalleryItems");

        _ = migrationBuilder.DropColumn(
            name: "DeletedBy",
            schema: "SiteContent",
            table: "GalleryItems");

        _ = migrationBuilder.DropColumn(
            name: "UpdatedBy",
            schema: "SiteContent",
            table: "GalleryItems");

        _ = migrationBuilder.DropColumn(
            name: "CreatedBy",
            schema: "SiteContent",
            table: "GalleryItemCategories");

        _ = migrationBuilder.DropColumn(
            name: "DeletedBy",
            schema: "SiteContent",
            table: "GalleryItemCategories");

        _ = migrationBuilder.DropColumn(
            name: "UpdatedBy",
            schema: "SiteContent",
            table: "GalleryItemCategories");

        _ = migrationBuilder.DropColumn(
            name: "CreatedBy",
            schema: "SiteContent",
            table: "GalleryCategories");

        _ = migrationBuilder.DropColumn(
            name: "DeletedBy",
            schema: "SiteContent",
            table: "GalleryCategories");

        _ = migrationBuilder.DropColumn(
            name: "UpdatedBy",
            schema: "SiteContent",
            table: "GalleryCategories");

        _ = migrationBuilder.DropColumn(
            name: "CreatedBy",
            schema: "SiteContent",
            table: "FooterInfos");

        _ = migrationBuilder.DropColumn(
            name: "DeletedBy",
            schema: "SiteContent",
            table: "FooterInfos");

        _ = migrationBuilder.DropColumn(
            name: "UpdatedBy",
            schema: "SiteContent",
            table: "FooterInfos");

        _ = migrationBuilder.DropColumn(
            name: "CreatedBy",
            schema: "SiteContent",
            table: "FeatureHighlights");

        _ = migrationBuilder.DropColumn(
            name: "DeletedBy",
            schema: "SiteContent",
            table: "FeatureHighlights");

        _ = migrationBuilder.DropColumn(
            name: "UpdatedBy",
            schema: "SiteContent",
            table: "FeatureHighlights");

        _ = migrationBuilder.DropColumn(
            name: "CreatedBy",
            schema: "SiteContent",
            table: "AboutUss");

        _ = migrationBuilder.DropColumn(
            name: "DeletedBy",
            schema: "SiteContent",
            table: "AboutUss");

        _ = migrationBuilder.DropColumn(
            name: "UpdatedBy",
            schema: "SiteContent",
            table: "AboutUss");
    }
}
