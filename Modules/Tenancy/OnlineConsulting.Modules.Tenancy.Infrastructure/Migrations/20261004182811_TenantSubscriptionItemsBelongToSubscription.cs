using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TenantSubscriptionItemsBelongToSubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_TenantSubscriptionItems_TenantSubscriptions_TenantSubscriptionId",
                schema: "Tenancy",
                table: "TenantSubscriptionItems",
                column: "TenantSubscriptionId",
                principalSchema: "Tenancy",
                principalTable: "TenantSubscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantSubscriptionItems_TenantSubscriptions_TenantSubscriptionId",
                schema: "Tenancy",
                table: "TenantSubscriptionItems");
        }
    }
}
