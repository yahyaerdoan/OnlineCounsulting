using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Inquiries.Infrastructure.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.EnsureSchema(
            name: "Inquiries");

        _ = migrationBuilder.CreateTable(
            name: "CompanyContacts",
            schema: "Inquiries",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                WorkingHours = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
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
                _ = table.PrimaryKey("PK_CompanyContacts", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "Messages",
            schema: "Inquiries",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
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
                _ = table.PrimaryKey("PK_Messages", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "NewsletterSubscribers",
            schema: "Inquiries",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
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
                _ = table.PrimaryKey("PK_NewsletterSubscribers", x => x.Id);
            });

        _ = migrationBuilder.CreateIndex(
            name: "IX_CompanyContacts_TenantId_DeletedDate",
            schema: "Inquiries",
            table: "CompanyContacts",
            columns: new[] { "TenantId", "DeletedDate" });

        _ = migrationBuilder.CreateIndex(
            name: "IX_Messages_TenantId_DeletedDate",
            schema: "Inquiries",
            table: "Messages",
            columns: new[] { "TenantId", "DeletedDate" });

        _ = migrationBuilder.CreateIndex(
            name: "IX_NewsletterSubscribers_Email",
            schema: "Inquiries",
            table: "NewsletterSubscribers",
            column: "Email");

        _ = migrationBuilder.CreateIndex(
            name: "IX_NewsletterSubscribers_TenantId_DeletedDate",
            schema: "Inquiries",
            table: "NewsletterSubscribers",
            columns: new[] { "TenantId", "DeletedDate" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropTable(
            name: "CompanyContacts",
            schema: "Inquiries");

        _ = migrationBuilder.DropTable(
            name: "Messages",
            schema: "Inquiries");

        _ = migrationBuilder.DropTable(
            name: "NewsletterSubscribers",
            schema: "Inquiries");
    }
}
