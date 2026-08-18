using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Scheduling.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentIdToWorkOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EquipmentId",
                schema: "Scheduling",
                table: "WorkOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_EquipmentId",
                schema: "Scheduling",
                table: "WorkOrders",
                column: "EquipmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_EquipmentId",
                schema: "Scheduling",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "EquipmentId",
                schema: "Scheduling",
                table: "WorkOrders");
        }
    }
}
