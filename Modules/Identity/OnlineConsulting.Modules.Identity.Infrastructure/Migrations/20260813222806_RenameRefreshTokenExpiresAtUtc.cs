using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameRefreshTokenExpiresAtUtc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefreshTokenExpiresAtUtc",
                schema: "identity",
                table: "AspNetUsers",
                newName: "RefreshTokenExpiresAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefreshTokenExpiresAt",
                schema: "identity",
                table: "AspNetUsers",
                newName: "RefreshTokenExpiresAtUtc");
        }
    }
}
