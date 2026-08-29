using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.EnsureSchema(
            name: "SiteContent");

        _ = migrationBuilder.CreateTable(
            name: "AboutUss",
            schema: "SiteContent",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                CoverImage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                VideoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                DisplayOrder = table.Column<int>(type: "int", nullable: false),
                Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                _ = table.PrimaryKey("PK_AboutUss", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "FaqItems",
            schema: "SiteContent",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Question = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                Answer = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
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
                _ = table.PrimaryKey("PK_FaqItems", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "FeatureHighlights",
            schema: "SiteContent",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                DisplayOrder = table.Column<int>(type: "int", nullable: false),
                Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                _ = table.PrimaryKey("PK_FeatureHighlights", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "FooterInfos",
            schema: "SiteContent",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                DisplayOrder = table.Column<int>(type: "int", nullable: false),
                Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                _ = table.PrimaryKey("PK_FooterInfos", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "GalleryCategories",
            schema: "SiteContent",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                _ = table.PrimaryKey("PK_GalleryCategories", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "GalleryItemCategories",
            schema: "SiteContent",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                GalleryItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                GalleryCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                _ = table.PrimaryKey("PK_GalleryItemCategories", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "GalleryItems",
            schema: "SiteContent",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                PhotoMediaAssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                DisplayOrder = table.Column<int>(type: "int", nullable: false),
                Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                _ = table.PrimaryKey("PK_GalleryItems", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "HeroSlides",
            schema: "SiteContent",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                DisplayOrder = table.Column<int>(type: "int", nullable: false),
                Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                _ = table.PrimaryKey("PK_HeroSlides", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "PageBanners",
            schema: "SiteContent",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                DisplayOrder = table.Column<int>(type: "int", nullable: false),
                Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                _ = table.PrimaryKey("PK_PageBanners", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "Partnerships",
            schema: "SiteContent",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                WebsiteUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                PhotoMediaAssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                DisplayOrder = table.Column<int>(type: "int", nullable: false),
                Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                _ = table.PrimaryKey("PK_Partnerships", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "PartnershipSocialLinks",
            schema: "SiteContent",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                PartnershipId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                Icon = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                IconColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
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
                _ = table.PrimaryKey("PK_PartnershipSocialLinks", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "Promotions",
            schema: "SiteContent",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                CtaText = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                CtaUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
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
                _ = table.PrimaryKey("PK_Promotions", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "ServiceAreas",
            schema: "SiteContent",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                Slug = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                IntroText = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
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
                _ = table.PrimaryKey("PK_ServiceAreas", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "ServiceOfferings",
            schema: "SiteContent",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                Icon = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                IconColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                DisplayOrder = table.Column<int>(type: "int", nullable: false),
                Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                _ = table.PrimaryKey("PK_ServiceOfferings", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "ServiceProcessSteps",
            schema: "SiteContent",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                Icon = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                IconColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                DisplayOrder = table.Column<int>(type: "int", nullable: false),
                Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                _ = table.PrimaryKey("PK_ServiceProcessSteps", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "SocialLinks",
            schema: "SiteContent",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                Icon = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                IconColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
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
                _ = table.PrimaryKey("PK_SocialLinks", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "Testimonials",
            schema: "SiteContent",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                DisplayOrder = table.Column<int>(type: "int", nullable: false),
                Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                _ = table.PrimaryKey("PK_Testimonials", x => x.Id);
            });

        _ = migrationBuilder.CreateIndex(
            name: "IX_AboutUss_TenantId_DeletedDate",
            schema: "SiteContent",
            table: "AboutUss",
            columns: new[] { "TenantId", "DeletedDate" });

        _ = migrationBuilder.CreateIndex(
            name: "IX_FaqItems_ServiceId",
            schema: "SiteContent",
            table: "FaqItems",
            column: "ServiceId");

        _ = migrationBuilder.CreateIndex(
            name: "IX_FaqItems_TenantId_DeletedDate",
            schema: "SiteContent",
            table: "FaqItems",
            columns: new[] { "TenantId", "DeletedDate" });

        _ = migrationBuilder.CreateIndex(
            name: "IX_FeatureHighlights_TenantId_DeletedDate",
            schema: "SiteContent",
            table: "FeatureHighlights",
            columns: new[] { "TenantId", "DeletedDate" });

        _ = migrationBuilder.CreateIndex(
            name: "IX_FooterInfos_TenantId_DeletedDate",
            schema: "SiteContent",
            table: "FooterInfos",
            columns: new[] { "TenantId", "DeletedDate" });

        _ = migrationBuilder.CreateIndex(
            name: "IX_GalleryCategories_TenantId_DeletedDate",
            schema: "SiteContent",
            table: "GalleryCategories",
            columns: new[] { "TenantId", "DeletedDate" });

        _ = migrationBuilder.CreateIndex(
            name: "IX_GalleryItemCategories_TenantId_DeletedDate",
            schema: "SiteContent",
            table: "GalleryItemCategories",
            columns: new[] { "TenantId", "DeletedDate" });

        _ = migrationBuilder.CreateIndex(
            name: "IX_GalleryItemCategories_TenantId_GalleryItemId_GalleryCategoryId",
            schema: "SiteContent",
            table: "GalleryItemCategories",
            columns: new[] { "TenantId", "GalleryItemId", "GalleryCategoryId" },
            unique: true);

        _ = migrationBuilder.CreateIndex(
            name: "IX_GalleryItems_TenantId_DeletedDate",
            schema: "SiteContent",
            table: "GalleryItems",
            columns: new[] { "TenantId", "DeletedDate" });

        _ = migrationBuilder.CreateIndex(
            name: "IX_HeroSlides_TenantId_DeletedDate",
            schema: "SiteContent",
            table: "HeroSlides",
            columns: new[] { "TenantId", "DeletedDate" });

        _ = migrationBuilder.CreateIndex(
            name: "IX_PageBanners_TenantId_DeletedDate",
            schema: "SiteContent",
            table: "PageBanners",
            columns: new[] { "TenantId", "DeletedDate" });

        _ = migrationBuilder.CreateIndex(
            name: "IX_Partnerships_TenantId_DeletedDate",
            schema: "SiteContent",
            table: "Partnerships",
            columns: new[] { "TenantId", "DeletedDate" });

        _ = migrationBuilder.CreateIndex(
            name: "IX_PartnershipSocialLinks_TenantId_DeletedDate",
            schema: "SiteContent",
            table: "PartnershipSocialLinks",
            columns: new[] { "TenantId", "DeletedDate" });

        _ = migrationBuilder.CreateIndex(
            name: "IX_Promotions_TenantId_DeletedDate",
            schema: "SiteContent",
            table: "Promotions",
            columns: new[] { "TenantId", "DeletedDate" });

        _ = migrationBuilder.CreateIndex(
            name: "IX_ServiceAreas_Slug",
            schema: "SiteContent",
            table: "ServiceAreas",
            column: "Slug",
            unique: true);

        _ = migrationBuilder.CreateIndex(
            name: "IX_ServiceAreas_TenantId_DeletedDate",
            schema: "SiteContent",
            table: "ServiceAreas",
            columns: new[] { "TenantId", "DeletedDate" });

        _ = migrationBuilder.CreateIndex(
            name: "IX_ServiceOfferings_TenantId_DeletedDate",
            schema: "SiteContent",
            table: "ServiceOfferings",
            columns: new[] { "TenantId", "DeletedDate" });

        _ = migrationBuilder.CreateIndex(
            name: "IX_ServiceProcessSteps_TenantId_DeletedDate",
            schema: "SiteContent",
            table: "ServiceProcessSteps",
            columns: new[] { "TenantId", "DeletedDate" });

        _ = migrationBuilder.CreateIndex(
            name: "IX_SocialLinks_TenantId_DeletedDate",
            schema: "SiteContent",
            table: "SocialLinks",
            columns: new[] { "TenantId", "DeletedDate" });

        _ = migrationBuilder.CreateIndex(
            name: "IX_Testimonials_TenantId_DeletedDate",
            schema: "SiteContent",
            table: "Testimonials",
            columns: new[] { "TenantId", "DeletedDate" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropTable(
            name: "AboutUss",
            schema: "SiteContent");

        _ = migrationBuilder.DropTable(
            name: "FaqItems",
            schema: "SiteContent");

        _ = migrationBuilder.DropTable(
            name: "FeatureHighlights",
            schema: "SiteContent");

        _ = migrationBuilder.DropTable(
            name: "FooterInfos",
            schema: "SiteContent");

        _ = migrationBuilder.DropTable(
            name: "GalleryCategories",
            schema: "SiteContent");

        _ = migrationBuilder.DropTable(
            name: "GalleryItemCategories",
            schema: "SiteContent");

        _ = migrationBuilder.DropTable(
            name: "GalleryItems",
            schema: "SiteContent");

        _ = migrationBuilder.DropTable(
            name: "HeroSlides",
            schema: "SiteContent");

        _ = migrationBuilder.DropTable(
            name: "PageBanners",
            schema: "SiteContent");

        _ = migrationBuilder.DropTable(
            name: "Partnerships",
            schema: "SiteContent");

        _ = migrationBuilder.DropTable(
            name: "PartnershipSocialLinks",
            schema: "SiteContent");

        _ = migrationBuilder.DropTable(
            name: "Promotions",
            schema: "SiteContent");

        _ = migrationBuilder.DropTable(
            name: "ServiceAreas",
            schema: "SiteContent");

        _ = migrationBuilder.DropTable(
            name: "ServiceOfferings",
            schema: "SiteContent");

        _ = migrationBuilder.DropTable(
            name: "ServiceProcessSteps",
            schema: "SiteContent");

        _ = migrationBuilder.DropTable(
            name: "SocialLinks",
            schema: "SiteContent");

        _ = migrationBuilder.DropTable(
            name: "Testimonials",
            schema: "SiteContent");
    }
}
