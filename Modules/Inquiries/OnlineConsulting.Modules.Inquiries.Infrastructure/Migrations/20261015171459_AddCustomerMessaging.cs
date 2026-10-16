using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Inquiries.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerMessaging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastMessageAt",
                schema: "Inquiries",
                table: "Messages",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "Inquiries",
                table: "Messages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFromCustomer",
                schema: "Inquiries",
                table: "MessageReplies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UserId",
                schema: "Inquiries",
                table: "Messages",
                column: "UserId");

            // Backfill: newest reply, else the original message. EXEC so idempotent scripts compile before the column exists.
            migrationBuilder.Sql("""
                EXEC(N'UPDATE m SET LastMessageAt = COALESCE(
                    (SELECT MAX(r.CreatedDate) FROM [Inquiries].[MessageReplies] r WHERE r.MessageId = m.Id),
                    m.RepliedAt, m.CreatedDate)
                FROM [Inquiries].[Messages] m;');
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_UserId",
                schema: "Inquiries",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "LastMessageAt",
                schema: "Inquiries",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "Inquiries",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "IsFromCustomer",
                schema: "Inquiries",
                table: "MessageReplies");
        }
    }
}
