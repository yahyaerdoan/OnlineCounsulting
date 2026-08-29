using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Referrals.Infrastructure.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.EnsureSchema(
            name: "Referrals");

        _ = migrationBuilder.CreateTable(
            name: "AccountCredits",
            schema: "Referrals",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Reason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                SourceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                SourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                _ = table.PrimaryKey("PK_AccountCredits", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "ReferralCodes",
            schema: "Referrals",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
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
                _ = table.PrimaryKey("PK_ReferralCodes", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "Referrals",
            schema: "Referrals",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ReferrerUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ReferredUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                RewardAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                RewardedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
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
                _ = table.PrimaryKey("PK_Referrals", x => x.Id);
            });

        _ = migrationBuilder.CreateIndex(
            name: "IX_AccountCredits_TenantId_DeletedDate",
            schema: "Referrals",
            table: "AccountCredits",
            columns: new[] { "TenantId", "DeletedDate" });

        _ = migrationBuilder.CreateIndex(
            name: "IX_AccountCredits_UserId",
            schema: "Referrals",
            table: "AccountCredits",
            column: "UserId");

        _ = migrationBuilder.CreateIndex(
            name: "IX_ReferralCodes_Code",
            schema: "Referrals",
            table: "ReferralCodes",
            column: "Code",
            unique: true);

        _ = migrationBuilder.CreateIndex(
            name: "IX_ReferralCodes_TenantId_DeletedDate",
            schema: "Referrals",
            table: "ReferralCodes",
            columns: new[] { "TenantId", "DeletedDate" });

        _ = migrationBuilder.CreateIndex(
            name: "IX_ReferralCodes_UserId",
            schema: "Referrals",
            table: "ReferralCodes",
            column: "UserId",
            unique: true);

        _ = migrationBuilder.CreateIndex(
            name: "IX_Referrals_ReferredUserId",
            schema: "Referrals",
            table: "Referrals",
            column: "ReferredUserId",
            unique: true);

        _ = migrationBuilder.CreateIndex(
            name: "IX_Referrals_ReferrerUserId",
            schema: "Referrals",
            table: "Referrals",
            column: "ReferrerUserId");

        _ = migrationBuilder.CreateIndex(
            name: "IX_Referrals_TenantId_DeletedDate",
            schema: "Referrals",
            table: "Referrals",
            columns: new[] { "TenantId", "DeletedDate" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropTable(
            name: "AccountCredits",
            schema: "Referrals");

        _ = migrationBuilder.DropTable(
            name: "ReferralCodes",
            schema: "Referrals");

        _ = migrationBuilder.DropTable(
            name: "Referrals",
            schema: "Referrals");
    }
}
