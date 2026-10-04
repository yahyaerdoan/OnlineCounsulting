using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GalleryItemOwnsCategoryLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GalleryItemCategories_TenantId_GalleryItemId_GalleryCategoryId",
                schema: "SiteContent",
                table: "GalleryItemCategories");

            migrationBuilder.CreateIndex(
                name: "IX_GalleryItemCategories_GalleryItemId",
                schema: "SiteContent",
                table: "GalleryItemCategories",
                column: "GalleryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_GalleryItemCategories_TenantId_GalleryItemId_GalleryCategoryId",
                schema: "SiteContent",
                table: "GalleryItemCategories",
                columns: new[] { "TenantId", "GalleryItemId", "GalleryCategoryId" },
                unique: true,
                filter: "[DeletedDate] IS NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_GalleryItemCategories_GalleryItems_GalleryItemId",
                schema: "SiteContent",
                table: "GalleryItemCategories",
                column: "GalleryItemId",
                principalSchema: "SiteContent",
                principalTable: "GalleryItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GalleryItemCategories_GalleryItems_GalleryItemId",
                schema: "SiteContent",
                table: "GalleryItemCategories");

            migrationBuilder.DropIndex(
                name: "IX_GalleryItemCategories_GalleryItemId",
                schema: "SiteContent",
                table: "GalleryItemCategories");

            migrationBuilder.DropIndex(
                name: "IX_GalleryItemCategories_TenantId_GalleryItemId_GalleryCategoryId",
                schema: "SiteContent",
                table: "GalleryItemCategories");

            migrationBuilder.CreateIndex(
                name: "IX_GalleryItemCategories_TenantId_GalleryItemId_GalleryCategoryId",
                schema: "SiteContent",
                table: "GalleryItemCategories",
                columns: new[] { "TenantId", "GalleryItemId", "GalleryCategoryId" },
                unique: true);
        }
    }
}
