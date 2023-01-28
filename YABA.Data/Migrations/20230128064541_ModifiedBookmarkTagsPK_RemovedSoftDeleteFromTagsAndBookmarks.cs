using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace YABA.Data.Migrations
{
    public partial class ModifiedBookmarkTagsPK_RemovedSoftDeleteFromTagsAndBookmarks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_bookmark_tags",
                table: "bookmark_tags");

            migrationBuilder.DropIndex(
                name: "ix_bookmark_tags_bookmark_id_tag_id",
                table: "bookmark_tags");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "bookmarks");

            migrationBuilder.DropColumn(
                name: "id",
                table: "bookmark_tags");

            migrationBuilder.AddPrimaryKey(
                name: "pk_bookmark_tags",
                table: "bookmark_tags",
                columns: new[] { "bookmark_id", "tag_id" });

            migrationBuilder.CreateIndex(
                name: "ix_tags_name",
                table: "tags",
                column: "name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_tags_name",
                table: "tags");

            migrationBuilder.DropPrimaryKey(
                name: "pk_bookmark_tags",
                table: "bookmark_tags");

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "tags",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "bookmarks",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "bookmark_tags",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "pk_bookmark_tags",
                table: "bookmark_tags",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_bookmark_tags_bookmark_id_tag_id",
                table: "bookmark_tags",
                columns: new[] { "bookmark_id", "tag_id" },
                unique: true);
        }
    }
}
