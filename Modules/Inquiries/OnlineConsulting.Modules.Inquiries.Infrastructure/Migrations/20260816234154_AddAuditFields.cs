using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Inquiries.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddAuditFields : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "Inquiries",
            table: "NewsletterSubscribers",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            schema: "Inquiries",
            table: "NewsletterSubscribers",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            schema: "Inquiries",
            table: "NewsletterSubscribers",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "Inquiries",
            table: "Messages",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            schema: "Inquiries",
            table: "Messages",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            schema: "Inquiries",
            table: "Messages",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "Inquiries",
            table: "CompanyContacts",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            schema: "Inquiries",
            table: "CompanyContacts",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            schema: "Inquiries",
            table: "CompanyContacts",
            type: "nvarchar(max)",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropColumn(
            name: "CreatedBy",
            schema: "Inquiries",
            table: "NewsletterSubscribers");

        _ = migrationBuilder.DropColumn(
            name: "DeletedBy",
            schema: "Inquiries",
            table: "NewsletterSubscribers");

        _ = migrationBuilder.DropColumn(
            name: "UpdatedBy",
            schema: "Inquiries",
            table: "NewsletterSubscribers");

        _ = migrationBuilder.DropColumn(
            name: "CreatedBy",
            schema: "Inquiries",
            table: "Messages");

        _ = migrationBuilder.DropColumn(
            name: "DeletedBy",
            schema: "Inquiries",
            table: "Messages");

        _ = migrationBuilder.DropColumn(
            name: "UpdatedBy",
            schema: "Inquiries",
            table: "Messages");

        _ = migrationBuilder.DropColumn(
            name: "CreatedBy",
            schema: "Inquiries",
            table: "CompanyContacts");

        _ = migrationBuilder.DropColumn(
            name: "DeletedBy",
            schema: "Inquiries",
            table: "CompanyContacts");

        _ = migrationBuilder.DropColumn(
            name: "UpdatedBy",
            schema: "Inquiries",
            table: "CompanyContacts");
    }
}
