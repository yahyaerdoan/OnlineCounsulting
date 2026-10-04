using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ServiceAreaSlugUniquePerTenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServiceAreas_Slug",
                schema: "SiteContent",
                table: "ServiceAreas");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAreas_TenantId_Slug",
                schema: "SiteContent",
                table: "ServiceAreas",
                columns: new[] { "TenantId", "Slug" },
                unique: true,
                filter: "[DeletedDate] IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServiceAreas_TenantId_Slug",
                schema: "SiteContent",
                table: "ServiceAreas");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAreas_Slug",
                schema: "SiteContent",
                table: "ServiceAreas",
                column: "Slug",
                unique: true);
        }
    }
}
