using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddFeatureHighlightsIntro : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.CreateTable(
            name: "FeatureHighlightsIntros",
            schema: "SiteContent",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                CoverMediaAssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                _ = table.PrimaryKey("PK_FeatureHighlightsIntros", x => x.Id);
            });

        _ = migrationBuilder.CreateIndex(
            name: "IX_FeatureHighlightsIntros_TenantId_DeletedDate",
            schema: "SiteContent",
            table: "FeatureHighlightsIntros",
            columns: new[] { "TenantId", "DeletedDate" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropTable(
            name: "FeatureHighlightsIntros",
            schema: "SiteContent");
    }
}
