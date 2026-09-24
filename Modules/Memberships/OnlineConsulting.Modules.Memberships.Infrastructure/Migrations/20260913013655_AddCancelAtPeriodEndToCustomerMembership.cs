using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Memberships.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddCancelAtPeriodEndToCustomerMembership : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.AddColumn<bool>(
            name: "CancelAtPeriodEnd",
            schema: "Memberships",
            table: "CustomerMemberships",
            type: "bit",
            nullable: false,
            defaultValue: false);

        _ = migrationBuilder.AddColumn<DateTimeOffset>(
            name: "PastDueSince",
            schema: "Memberships",
            table: "CustomerMemberships",
            type: "datetimeoffset",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropColumn(
            name: "CancelAtPeriodEnd",
            schema: "Memberships",
            table: "CustomerMemberships");

        _ = migrationBuilder.DropColumn(
            name: "PastDueSince",
            schema: "Memberships",
            table: "CustomerMemberships");
    }
}
