using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Inquiries.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCcAndSourceReferenceToOutboxEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cc",
                schema: "Notifications",
                table: "OutboxEmails",
                type: "nvarchar(320)",
                maxLength: 320,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceReference",
                schema: "Notifications",
                table: "OutboxEmails",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cc",
                schema: "Notifications",
                table: "OutboxEmails");

            migrationBuilder.DropColumn(
                name: "SourceReference",
                schema: "Notifications",
                table: "OutboxEmails");
        }
    }
}
