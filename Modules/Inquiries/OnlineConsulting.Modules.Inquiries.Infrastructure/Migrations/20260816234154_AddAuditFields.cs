using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Inquiries.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Inquiries",
                table: "NewsletterSubscribers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "Inquiries",
                table: "NewsletterSubscribers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "Inquiries",
                table: "NewsletterSubscribers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Inquiries",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "Inquiries",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "Inquiries",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Inquiries",
                table: "CompanyContacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "Inquiries",
                table: "CompanyContacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "Inquiries",
                table: "CompanyContacts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Inquiries",
                table: "NewsletterSubscribers");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "Inquiries",
                table: "NewsletterSubscribers");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "Inquiries",
                table: "NewsletterSubscribers");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Inquiries",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "Inquiries",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "Inquiries",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Inquiries",
                table: "CompanyContacts");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "Inquiries",
                table: "CompanyContacts");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "Inquiries",
                table: "CompanyContacts");
        }
    }
}
