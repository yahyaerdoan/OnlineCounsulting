using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.FeatureFlags.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "FeatureFlags",
                table: "FeatureFlags",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "FeatureFlags",
                table: "FeatureFlags",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "FeatureFlags",
                table: "FeatureFlags",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "FeatureFlags",
                table: "FeatureFlags");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "FeatureFlags",
                table: "FeatureFlags");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "FeatureFlags",
                table: "FeatureFlags");
        }
    }
}
