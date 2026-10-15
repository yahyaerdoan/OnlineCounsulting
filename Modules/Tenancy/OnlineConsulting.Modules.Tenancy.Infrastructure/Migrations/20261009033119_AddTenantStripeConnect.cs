using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantStripeConnect : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "StripeChargesEnabled",
                schema: "Tenancy",
                table: "Tenants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "StripeConnectedAccountId",
                schema: "Tenancy",
                table: "Tenants",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "StripePayoutsEnabled",
                schema: "Tenancy",
                table: "Tenants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_StripeConnectedAccountId",
                schema: "Tenancy",
                table: "Tenants",
                column: "StripeConnectedAccountId",
                unique: true,
                filter: "[StripeConnectedAccountId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tenants_StripeConnectedAccountId",
                schema: "Tenancy",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "StripeChargesEnabled",
                schema: "Tenancy",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "StripeConnectedAccountId",
                schema: "Tenancy",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "StripePayoutsEnabled",
                schema: "Tenancy",
                table: "Tenants");
        }
    }
}
