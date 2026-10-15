using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TenantScopedUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_Invites_TenantId_DeletedDate",
                schema: "Identity",
                table: "Invites",
                columns: new[] { "TenantId", "DeletedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TenantId_DeletedDate",
                schema: "Identity",
                table: "AspNetUsers",
                columns: new[] { "TenantId", "DeletedDate" });

            migrationBuilder.CreateIndex(
                name: "TenantUserNameIndex",
                schema: "Identity",
                table: "AspNetUsers",
                columns: new[] { "TenantId", "NormalizedUserName" },
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "Identity",
                table: "AspNetUsers",
                column: "NormalizedUserName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invites_TenantId_DeletedDate",
                schema: "Identity",
                table: "Invites");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TenantId_DeletedDate",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "TenantUserNameIndex",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "Identity",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }
    }
}
