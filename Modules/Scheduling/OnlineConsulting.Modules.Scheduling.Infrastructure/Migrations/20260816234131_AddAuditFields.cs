using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Scheduling.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddAuditFields : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "Notifications",
            table: "OutboxEmails",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            schema: "Notifications",
            table: "OutboxEmails",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            schema: "Notifications",
            table: "OutboxEmails",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "Scheduling",
            table: "AvailabilityRules",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            schema: "Scheduling",
            table: "AvailabilityRules",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            schema: "Scheduling",
            table: "AvailabilityRules",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            schema: "Scheduling",
            table: "Appointments",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            schema: "Scheduling",
            table: "Appointments",
            type: "nvarchar(max)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            schema: "Scheduling",
            table: "Appointments",
            type: "nvarchar(max)",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropColumn(
            name: "CreatedBy",
            schema: "Notifications",
            table: "OutboxEmails");

        _ = migrationBuilder.DropColumn(
            name: "DeletedBy",
            schema: "Notifications",
            table: "OutboxEmails");

        _ = migrationBuilder.DropColumn(
            name: "UpdatedBy",
            schema: "Notifications",
            table: "OutboxEmails");

        _ = migrationBuilder.DropColumn(
            name: "CreatedBy",
            schema: "Scheduling",
            table: "AvailabilityRules");

        _ = migrationBuilder.DropColumn(
            name: "DeletedBy",
            schema: "Scheduling",
            table: "AvailabilityRules");

        _ = migrationBuilder.DropColumn(
            name: "UpdatedBy",
            schema: "Scheduling",
            table: "AvailabilityRules");

        _ = migrationBuilder.DropColumn(
            name: "CreatedBy",
            schema: "Scheduling",
            table: "Appointments");

        _ = migrationBuilder.DropColumn(
            name: "DeletedBy",
            schema: "Scheduling",
            table: "Appointments");

        _ = migrationBuilder.DropColumn(
            name: "UpdatedBy",
            schema: "Scheduling",
            table: "Appointments");
    }
}
