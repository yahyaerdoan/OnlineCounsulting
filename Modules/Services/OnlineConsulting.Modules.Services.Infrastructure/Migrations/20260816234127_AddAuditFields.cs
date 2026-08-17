using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Services.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Services",
                table: "Services",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "Services",
                table: "Services",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "Services",
                table: "Services",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Services",
                table: "ServiceMediaItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "Services",
                table: "ServiceMediaItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "Services",
                table: "ServiceMediaItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Services",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "Services",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "Services",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Services",
                table: "ServiceMediaItems");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "Services",
                table: "ServiceMediaItems");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "Services",
                table: "ServiceMediaItems");
        }
    }
}
