using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Services.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCoverMediaAssetToService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CoverMediaAssetId",
                schema: "Services",
                table: "Services",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverMediaAssetId",
                schema: "Services",
                table: "Services");
        }
    }
}
