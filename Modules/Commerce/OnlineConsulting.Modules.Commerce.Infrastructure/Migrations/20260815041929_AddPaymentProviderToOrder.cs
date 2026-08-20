using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Commerce.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddPaymentProviderToOrder : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.AddColumn<string>(
            name: "PaymentProvider",
            schema: "Commerce",
            table: "Orders",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "ProviderPaymentId",
            schema: "Commerce",
            table: "Orders",
            type: "nvarchar(200)",
            maxLength: 200,
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropColumn(
            name: "PaymentProvider",
            schema: "Commerce",
            table: "Orders");

        _ = migrationBuilder.DropColumn(
            name: "ProviderPaymentId",
            schema: "Commerce",
            table: "Orders");
    }
}
