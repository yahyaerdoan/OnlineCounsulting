using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantTimeZone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TimeZoneId",
                schema: "Tenancy",
                table: "Tenants",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "America/Chicago");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeZoneId",
                schema: "Tenancy",
                table: "Tenants");
        }
    }
}
