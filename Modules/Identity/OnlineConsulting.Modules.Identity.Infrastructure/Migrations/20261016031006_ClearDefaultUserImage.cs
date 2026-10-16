using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ClearDefaultUserImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Default image and API-disk photos are gone; initials are shown instead.
            migrationBuilder.Sql("UPDATE [Identity].[AspNetUsers] SET [ImageUrl] = NULL WHERE [ImageUrl] LIKE '/Resource/LocalStorage/%';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data-only: cleared paths cannot be restored.
        }
    }
}
