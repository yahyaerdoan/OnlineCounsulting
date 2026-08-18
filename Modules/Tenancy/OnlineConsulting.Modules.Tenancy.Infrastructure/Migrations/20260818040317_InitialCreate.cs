using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "Tenancy");

        migrationBuilder.CreateTable(
            name: "Bundles",
            schema: "Tenancy",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                ModuleKeys = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IsPubliclyVisible = table.Column<bool>(type: "bit", nullable: false),
                CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DeletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Bundles", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ModuleOfferings",
            schema: "Tenancy",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                BillingCycle = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                ProviderProductId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                ProviderPriceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                IsPubliclyVisible = table.Column<bool>(type: "bit", nullable: false),
                CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DeletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ModuleOfferings", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Tenants",
            schema: "Tenancy",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Slug = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                PrimaryContactEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                ProviderCustomerId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DeletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Tenants", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "TenantSubscriptionItems",
            schema: "Tenancy",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TenantSubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ModuleKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                ProviderSubscriptionItemId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                PriceAtAddition = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DeletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TenantSubscriptionItems", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "TenantSubscriptions",
            schema: "Tenancy",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                RenewalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                ProviderSubscriptionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DeletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TenantSubscriptions", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ModuleOfferings_Key",
            schema: "Tenancy",
            table: "ModuleOfferings",
            column: "Key",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Tenants_Slug",
            schema: "Tenancy",
            table: "Tenants",
            column: "Slug",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_TenantSubscriptionItems_TenantSubscriptionId",
            schema: "Tenancy",
            table: "TenantSubscriptionItems",
            column: "TenantSubscriptionId");

        migrationBuilder.CreateIndex(
            name: "IX_TenantSubscriptions_TenantId",
            schema: "Tenancy",
            table: "TenantSubscriptions",
            column: "TenantId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Bundles",
            schema: "Tenancy");

        migrationBuilder.DropTable(
            name: "ModuleOfferings",
            schema: "Tenancy");

        migrationBuilder.DropTable(
            name: "Tenants",
            schema: "Tenancy");

        migrationBuilder.DropTable(
            name: "TenantSubscriptionItems",
            schema: "Tenancy");

        migrationBuilder.DropTable(
            name: "TenantSubscriptions",
            schema: "Tenancy");
    }
}
