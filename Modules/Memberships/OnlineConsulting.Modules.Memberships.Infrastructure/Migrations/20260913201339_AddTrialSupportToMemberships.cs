using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Memberships.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTrialSupportToMemberships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TrialDays",
                schema: "Memberships",
                table: "MembershipPlans",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "TrialEndDate",
                schema: "Memberships",
                table: "CustomerMemberships",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrialDays",
                schema: "Memberships",
                table: "MembershipPlans");

            migrationBuilder.DropColumn(
                name: "TrialEndDate",
                schema: "Memberships",
                table: "CustomerMemberships");
        }
    }
}
