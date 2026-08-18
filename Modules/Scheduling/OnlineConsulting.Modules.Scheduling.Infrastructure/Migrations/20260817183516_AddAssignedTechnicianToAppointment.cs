using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Scheduling.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedTechnicianToAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AssignedTechnicianUserId",
                schema: "Scheduling",
                table: "Appointments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AssignedTechnicianUserId",
                schema: "Scheduling",
                table: "Appointments",
                column: "AssignedTechnicianUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Appointments_AssignedTechnicianUserId",
                schema: "Scheduling",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "AssignedTechnicianUserId",
                schema: "Scheduling",
                table: "Appointments");
        }
    }
}
