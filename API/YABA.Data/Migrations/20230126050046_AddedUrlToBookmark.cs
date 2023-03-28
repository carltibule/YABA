using Microsoft.EntityFrameworkCore.Migrations;

namespace YABA.Data.Migrations
{
    public partial class AddedUrlToBookmark : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "url",
                table: "bookmarks",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "url",
                table: "bookmarks");
        }
    }
}
