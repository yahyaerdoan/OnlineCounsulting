using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.DataAccess.Migrations;

/// <inheritdoc />
public partial class AddTenantIdToUser : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "TenantId",
            table: "AspNetUsers",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000001"));
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "TenantId",
            table: "AspNetUsers");
    }
}
