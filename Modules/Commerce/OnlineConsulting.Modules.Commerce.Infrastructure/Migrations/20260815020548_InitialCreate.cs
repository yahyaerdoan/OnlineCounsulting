using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Commerce.Infrastructure.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.EnsureSchema(
            name: "Commerce");

        _ = migrationBuilder.CreateTable(
            name: "BasketItems",
            schema: "Commerce",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                BasketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Quantity = table.Column<int>(type: "int", nullable: false),
                Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                TaxRate = table.Column<int>(type: "int", nullable: false),
                TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                SubTotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                DeletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_BasketItems", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "Baskets",
            schema: "Commerce",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                GuestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                Quantity = table.Column<int>(type: "int", nullable: false),
                SubTotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                DeletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_Baskets", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "OrderItems",
            schema: "Commerce",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Quantity = table.Column<int>(type: "int", nullable: false),
                UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                TaxRate = table.Column<int>(type: "int", nullable: false),
                TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                SubTotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                DeletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_OrderItems", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "Orders",
            schema: "Commerce",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OrderNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                OrderStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                PaymentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ShippingAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                InvoiceAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                DeletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_Orders", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "UserAddresses",
            schema: "Commerce",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                AddressName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                AddressLine = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Zipcode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                IsShippingAddress = table.Column<bool>(type: "bit", nullable: false),
                IsBillingAddress = table.Column<bool>(type: "bit", nullable: false),
                CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                DeletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_UserAddresses", x => x.Id);
            });

        _ = migrationBuilder.CreateIndex(
            name: "IX_BasketItems_BasketId",
            schema: "Commerce",
            table: "BasketItems",
            column: "BasketId");

        _ = migrationBuilder.CreateIndex(
            name: "IX_Baskets_GuestId",
            schema: "Commerce",
            table: "Baskets",
            column: "GuestId");

        _ = migrationBuilder.CreateIndex(
            name: "IX_Baskets_UserId",
            schema: "Commerce",
            table: "Baskets",
            column: "UserId");

        _ = migrationBuilder.CreateIndex(
            name: "IX_OrderItems_OrderId",
            schema: "Commerce",
            table: "OrderItems",
            column: "OrderId");

        _ = migrationBuilder.CreateIndex(
            name: "IX_Orders_UserId",
            schema: "Commerce",
            table: "Orders",
            column: "UserId");

        _ = migrationBuilder.CreateIndex(
            name: "IX_UserAddresses_UserId",
            schema: "Commerce",
            table: "UserAddresses",
            column: "UserId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropTable(
            name: "BasketItems",
            schema: "Commerce");

        _ = migrationBuilder.DropTable(
            name: "Baskets",
            schema: "Commerce");

        _ = migrationBuilder.DropTable(
            name: "OrderItems",
            schema: "Commerce");

        _ = migrationBuilder.DropTable(
            name: "Orders",
            schema: "Commerce");

        _ = migrationBuilder.DropTable(
            name: "UserAddresses",
            schema: "Commerce");
    }
}
