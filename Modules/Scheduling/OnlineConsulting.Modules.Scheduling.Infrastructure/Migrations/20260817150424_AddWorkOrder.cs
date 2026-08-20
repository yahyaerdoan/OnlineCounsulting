using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Scheduling.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddWorkOrder : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.CreateTable(
            name: "WorkOrderMediaItems",
            schema: "Scheduling",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                WorkOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                MediaAssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                IsBeforePhoto = table.Column<bool>(type: "bit", nullable: false),
                DisplayOrder = table.Column<int>(type: "int", nullable: false),
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
                _ = table.PrimaryKey("PK_WorkOrderMediaItems", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "WorkOrders",
            schema: "Scheduling",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                AppointmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TechnicianUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                PartsUsed = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                TechnicianNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                CompletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
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
                _ = table.PrimaryKey("PK_WorkOrders", x => x.Id);
            });

        _ = migrationBuilder.CreateIndex(
            name: "IX_WorkOrderMediaItems_WorkOrderId",
            schema: "Scheduling",
            table: "WorkOrderMediaItems",
            column: "WorkOrderId");

        _ = migrationBuilder.CreateIndex(
            name: "IX_WorkOrders_AppointmentId",
            schema: "Scheduling",
            table: "WorkOrders",
            column: "AppointmentId",
            unique: true);

        _ = migrationBuilder.CreateIndex(
            name: "IX_WorkOrders_TechnicianUserId",
            schema: "Scheduling",
            table: "WorkOrders",
            column: "TechnicianUserId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropTable(
            name: "WorkOrderMediaItems",
            schema: "Scheduling");

        _ = migrationBuilder.DropTable(
            name: "WorkOrders",
            schema: "Scheduling");
    }
}
