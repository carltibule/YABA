using Microsoft.EntityFrameworkCore.Migrations;

namespace YABA.Data.Migrations
{
    public partial class AddUniqueNameAndUserIdComboConstraintToTagsTable_AddUniqueUrlAndUserIdComboConstraintToBookmark : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_tags_name",
                table: "tags");

            migrationBuilder.CreateIndex(
                name: "ix_tags_name_user_id",
                table: "tags",
                columns: new[] { "name", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_bookmarks_url_user_id",
                table: "bookmarks",
                columns: new[] { "url", "user_id" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_tags_name_user_id",
                table: "tags");

            migrationBuilder.DropIndex(
                name: "ix_bookmarks_url_user_id",
                table: "bookmarks");

            migrationBuilder.CreateIndex(
                name: "ix_tags_name",
                table: "tags",
                column: "name",
                unique: true);
        }
    }
}
