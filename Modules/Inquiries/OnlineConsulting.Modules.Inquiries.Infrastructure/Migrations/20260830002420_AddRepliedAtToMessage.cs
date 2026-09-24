using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Inquiries.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddRepliedAtToMessage : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.AddColumn<DateTimeOffset>(
            name: "RepliedAt",
            schema: "Inquiries",
            table: "Messages",
            type: "datetimeoffset",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropColumn(
            name: "RepliedAt",
            schema: "Inquiries",
            table: "Messages");
    }
}
