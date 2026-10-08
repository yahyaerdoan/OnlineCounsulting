using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TenantSoftDeleteAndLiveSlugUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tenants_Slug",
                schema: "Tenancy",
                table: "Tenants");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Slug",
                schema: "Tenancy",
                table: "Tenants",
                column: "Slug",
                unique: true,
                filter: "[DeletedDate] IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tenants_Slug",
                schema: "Tenancy",
                table: "Tenants");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Slug",
                schema: "Tenancy",
                table: "Tenants",
                column: "Slug",
                unique: true);
        }
    }
}
