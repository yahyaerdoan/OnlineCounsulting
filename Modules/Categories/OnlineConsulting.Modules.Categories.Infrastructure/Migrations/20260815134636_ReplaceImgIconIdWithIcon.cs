using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineConsulting.Modules.Categories.Infrastructure.Migrations;

/// <inheritdoc />
public partial class ReplaceImgIconIdWithIcon : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ImgIconId",
            schema: "Categories",
            table: "Categories");

        migrationBuilder.AddColumn<string>(
            name: "Icon",
            schema: "Categories",
            table: "Categories",
            type: "nvarchar(2000)",
            maxLength: 2000,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "IconColor",
            schema: "Categories",
            table: "Categories",
            type: "nvarchar(7)",
            maxLength: 7,
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Icon",
            schema: "Categories",
            table: "Categories");

        migrationBuilder.DropColumn(
            name: "IconColor",
            schema: "Categories",
            table: "Categories");

        migrationBuilder.AddColumn<Guid>(
            name: "ImgIconId",
            schema: "Categories",
            table: "Categories",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
    }
}
