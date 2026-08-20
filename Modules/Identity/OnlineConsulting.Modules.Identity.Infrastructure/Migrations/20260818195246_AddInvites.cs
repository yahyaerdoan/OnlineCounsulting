using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Identity.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddInvites : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.CreateTable(
            name: "Invites",
            schema: "Identity",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Token = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                RoleName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                InvitedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                AcceptedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DeletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_Invites", x => x.Id);
            });

        _ = migrationBuilder.CreateIndex(
            name: "IX_Invites_TenantId_Email",
            schema: "Identity",
            table: "Invites",
            columns: new[] { "TenantId", "Email" });

        _ = migrationBuilder.CreateIndex(
            name: "IX_Invites_Token",
            schema: "Identity",
            table: "Invites",
            column: "Token",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropTable(
            name: "Invites",
            schema: "Identity");
    }
}
