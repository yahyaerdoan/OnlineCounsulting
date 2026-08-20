using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Services.Infrastructure.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.EnsureSchema(
            name: "Services");

        _ = migrationBuilder.CreateTable(
            name: "Services",
            schema: "Services",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Slug = table.Column<string>(type: "nvarchar(220)", maxLength: 220, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                DetailedDescription = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                FeaturedArea = table.Column<bool>(type: "bit", nullable: false),
                DiscountRate = table.Column<int>(type: "int", nullable: false),
                TaxRate = table.Column<int>(type: "int", nullable: false),
                DiscountedPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                DeletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_Services", x => x.Id);
            });

        _ = migrationBuilder.CreateIndex(
            name: "IX_Services_CategoryId",
            schema: "Services",
            table: "Services",
            column: "CategoryId");

        _ = migrationBuilder.CreateIndex(
            name: "IX_Services_TenantId_Slug",
            schema: "Services",
            table: "Services",
            columns: new[] { "TenantId", "Slug" },
            unique: true,
            filter: "[DeletedDate] IS NULL");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropTable(
            name: "Services",
            schema: "Services");
    }
}
