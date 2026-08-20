using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Scheduling.Infrastructure.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.EnsureSchema(
            name: "Scheduling");

        _ = migrationBuilder.CreateTable(
            name: "Appointments",
            schema: "Scheduling",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                ScheduledStart = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                ScheduledEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                CustomerNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                RequiresPrepayment = table.Column<bool>(type: "bit", nullable: false),
                CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                DeletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_Appointments", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "AvailabilityRules",
            schema: "Scheduling",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                DayOfWeek = table.Column<int>(type: "int", nullable: false),
                StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                SlotDurationMinutes = table.Column<int>(type: "int", nullable: false),
                CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                DeletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_AvailabilityRules", x => x.Id);
            });

        _ = migrationBuilder.CreateIndex(
            name: "IX_Appointments_ServiceId",
            schema: "Scheduling",
            table: "Appointments",
            column: "ServiceId");

        _ = migrationBuilder.CreateIndex(
            name: "IX_Appointments_TenantId_ScheduledStart",
            schema: "Scheduling",
            table: "Appointments",
            columns: new[] { "TenantId", "ScheduledStart" });

        _ = migrationBuilder.CreateIndex(
            name: "IX_Appointments_UserId",
            schema: "Scheduling",
            table: "Appointments",
            column: "UserId");

        _ = migrationBuilder.CreateIndex(
            name: "IX_AvailabilityRules_TenantId_DayOfWeek",
            schema: "Scheduling",
            table: "AvailabilityRules",
            columns: new[] { "TenantId", "DayOfWeek" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropTable(
            name: "Appointments",
            schema: "Scheduling");

        _ = migrationBuilder.DropTable(
            name: "AvailabilityRules",
            schema: "Scheduling");
    }
}
