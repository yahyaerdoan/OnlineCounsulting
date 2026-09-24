using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Memberships.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddTrialSupportToMemberships : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.AddColumn<int>(
            name: "TrialDays",
            schema: "Memberships",
            table: "MembershipPlans",
            type: "int",
            nullable: true);

        _ = migrationBuilder.AddColumn<DateTimeOffset>(
            name: "TrialEndDate",
            schema: "Memberships",
            table: "CustomerMemberships",
            type: "datetimeoffset",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropColumn(
            name: "TrialDays",
            schema: "Memberships",
            table: "MembershipPlans");

        _ = migrationBuilder.DropColumn(
            name: "TrialEndDate",
            schema: "Memberships",
            table: "CustomerMemberships");
    }
}
