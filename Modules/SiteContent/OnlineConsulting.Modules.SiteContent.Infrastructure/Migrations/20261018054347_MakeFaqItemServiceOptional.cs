using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeFaqItemServiceOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SQL Server can't alter a column an index depends on, so the index is dropped and recreated around it.
            migrationBuilder.DropIndex(
                name: "IX_FaqItems_ServiceId",
                schema: "SiteContent",
                table: "FaqItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "ServiceId",
                schema: "SiteContent",
                table: "FaqItems",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_FaqItems_ServiceId",
                schema: "SiteContent",
                table: "FaqItems",
                column: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FaqItems_ServiceId",
                schema: "SiteContent",
                table: "FaqItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "ServiceId",
                schema: "SiteContent",
                table: "FaqItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FaqItems_ServiceId",
                schema: "SiteContent",
                table: "FaqItems",
                column: "ServiceId");
        }
    }
}
