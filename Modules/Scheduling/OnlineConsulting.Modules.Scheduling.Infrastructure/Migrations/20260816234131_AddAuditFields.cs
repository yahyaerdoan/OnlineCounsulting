using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Scheduling.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Notifications",
                table: "OutboxEmails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "Notifications",
                table: "OutboxEmails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "Notifications",
                table: "OutboxEmails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Scheduling",
                table: "AvailabilityRules",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "Scheduling",
                table: "AvailabilityRules",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "Scheduling",
                table: "AvailabilityRules",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Scheduling",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "Scheduling",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "Scheduling",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Notifications",
                table: "OutboxEmails");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "Notifications",
                table: "OutboxEmails");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "Notifications",
                table: "OutboxEmails");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Scheduling",
                table: "AvailabilityRules");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "Scheduling",
                table: "AvailabilityRules");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "Scheduling",
                table: "AvailabilityRules");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Scheduling",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "Scheduling",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "Scheduling",
                table: "Appointments");
        }
    }
}
