using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Memberships.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddPromoCodesToMemberships : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.AddColumn<Guid>(
            name: "PromoCodeId",
            schema: "Memberships",
            table: "CustomerMemberships",
            type: "uniqueidentifier",
            nullable: true);

        _ = migrationBuilder.CreateTable(
            name: "PromoCodes",
            schema: "Memberships",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                DiscountType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                DiscountValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                MaxRedemptions = table.Column<int>(type: "int", nullable: true),
                RedemptionCount = table.Column<int>(type: "int", nullable: false),
                ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                MembershipPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                _ = table.PrimaryKey("PK_PromoCodes", x => x.Id);
            });

        _ = migrationBuilder.CreateIndex(
            name: "IX_PromoCodes_TenantId_Code",
            schema: "Memberships",
            table: "PromoCodes",
            columns: new[] { "TenantId", "Code" },
            unique: true);

        _ = migrationBuilder.CreateIndex(
            name: "IX_PromoCodes_TenantId_DeletedDate",
            schema: "Memberships",
            table: "PromoCodes",
            columns: new[] { "TenantId", "DeletedDate" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropTable(
            name: "PromoCodes",
            schema: "Memberships");

        _ = migrationBuilder.DropColumn(
            name: "PromoCodeId",
            schema: "Memberships",
            table: "CustomerMemberships");
    }
}
