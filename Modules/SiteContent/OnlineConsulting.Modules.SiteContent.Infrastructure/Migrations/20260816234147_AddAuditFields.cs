using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "SiteContent",
                table: "Testimonials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "SiteContent",
                table: "Testimonials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "SiteContent",
                table: "Testimonials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "SiteContent",
                table: "SocialLinks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "SiteContent",
                table: "SocialLinks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "SiteContent",
                table: "SocialLinks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "SiteContent",
                table: "ServiceProcessSteps",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "SiteContent",
                table: "ServiceProcessSteps",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "SiteContent",
                table: "ServiceProcessSteps",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "SiteContent",
                table: "ServiceOfferings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "SiteContent",
                table: "ServiceOfferings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "SiteContent",
                table: "ServiceOfferings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "SiteContent",
                table: "PartnershipSocialLinks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "SiteContent",
                table: "PartnershipSocialLinks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "SiteContent",
                table: "PartnershipSocialLinks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "SiteContent",
                table: "Partnerships",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "SiteContent",
                table: "Partnerships",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "SiteContent",
                table: "Partnerships",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "SiteContent",
                table: "PageBanners",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "SiteContent",
                table: "PageBanners",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "SiteContent",
                table: "PageBanners",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "SiteContent",
                table: "HeroSlides",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "SiteContent",
                table: "HeroSlides",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "SiteContent",
                table: "HeroSlides",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "SiteContent",
                table: "GalleryItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "SiteContent",
                table: "GalleryItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "SiteContent",
                table: "GalleryItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "SiteContent",
                table: "GalleryItemCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "SiteContent",
                table: "GalleryItemCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "SiteContent",
                table: "GalleryItemCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "SiteContent",
                table: "GalleryCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "SiteContent",
                table: "GalleryCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "SiteContent",
                table: "GalleryCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "SiteContent",
                table: "FooterInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "SiteContent",
                table: "FooterInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "SiteContent",
                table: "FooterInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "SiteContent",
                table: "FeatureHighlights",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "SiteContent",
                table: "FeatureHighlights",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "SiteContent",
                table: "FeatureHighlights",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "SiteContent",
                table: "AboutUss",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "SiteContent",
                table: "AboutUss",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "SiteContent",
                table: "AboutUss",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "SiteContent",
                table: "Testimonials");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "SiteContent",
                table: "Testimonials");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "SiteContent",
                table: "Testimonials");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "SiteContent",
                table: "SocialLinks");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "SiteContent",
                table: "SocialLinks");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "SiteContent",
                table: "SocialLinks");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "SiteContent",
                table: "ServiceProcessSteps");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "SiteContent",
                table: "ServiceProcessSteps");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "SiteContent",
                table: "ServiceProcessSteps");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "SiteContent",
                table: "ServiceOfferings");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "SiteContent",
                table: "ServiceOfferings");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "SiteContent",
                table: "ServiceOfferings");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "SiteContent",
                table: "PartnershipSocialLinks");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "SiteContent",
                table: "PartnershipSocialLinks");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "SiteContent",
                table: "PartnershipSocialLinks");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "SiteContent",
                table: "Partnerships");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "SiteContent",
                table: "Partnerships");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "SiteContent",
                table: "Partnerships");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "SiteContent",
                table: "PageBanners");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "SiteContent",
                table: "PageBanners");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "SiteContent",
                table: "PageBanners");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "SiteContent",
                table: "HeroSlides");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "SiteContent",
                table: "HeroSlides");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "SiteContent",
                table: "HeroSlides");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "SiteContent",
                table: "GalleryItems");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "SiteContent",
                table: "GalleryItems");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "SiteContent",
                table: "GalleryItems");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "SiteContent",
                table: "GalleryItemCategories");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "SiteContent",
                table: "GalleryItemCategories");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "SiteContent",
                table: "GalleryItemCategories");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "SiteContent",
                table: "GalleryCategories");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "SiteContent",
                table: "GalleryCategories");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "SiteContent",
                table: "GalleryCategories");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "SiteContent",
                table: "FooterInfos");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "SiteContent",
                table: "FooterInfos");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "SiteContent",
                table: "FooterInfos");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "SiteContent",
                table: "FeatureHighlights");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "SiteContent",
                table: "FeatureHighlights");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "SiteContent",
                table: "FeatureHighlights");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "SiteContent",
                table: "AboutUss");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "SiteContent",
                table: "AboutUss");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "SiteContent",
                table: "AboutUss");
        }
    }
}
