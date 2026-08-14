using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserStatusToIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                schema: "auth",
                table: "AspNetUsers",
                newName: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsActive",
                schema: "auth",
                table: "AspNetUsers",
                newName: "Status");
        }
    }
}
