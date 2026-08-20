using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Services.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddServicePriceType : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.AddColumn<decimal>(
            name: "PriceMax",
            schema: "Services",
            table: "Services",
            type: "decimal(18,2)",
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "PriceType",
            schema: "Services",
            table: "Services",
            type: "nvarchar(20)",
            maxLength: 20,
            nullable: false,
            defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropColumn(
            name: "PriceMax",
            schema: "Services",
            table: "Services");

        _ = migrationBuilder.DropColumn(
            name: "PriceType",
            schema: "Services",
            table: "Services");
    }
}
