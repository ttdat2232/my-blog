using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBlog.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureCommentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comments_comments_parent_comment_id",
                table: "comments");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "subscriptions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 6, 1, 8, 41, 49, 892, DateTimeKind.Utc).AddTicks(9629),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 5, 25, 10, 31, 11, 16, DateTimeKind.Utc).AddTicks(6479));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 6, 1, 8, 41, 49, 892, DateTimeKind.Utc).AddTicks(8204),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 5, 25, 10, 31, 11, 16, DateTimeKind.Utc).AddTicks(5341));

            migrationBuilder.AddForeignKey(
                name: "FK_comments_comments_parent_comment_id",
                table: "comments",
                column: "parent_comment_id",
                principalTable: "comments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comments_comments_parent_comment_id",
                table: "comments");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "subscriptions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 5, 25, 10, 31, 11, 16, DateTimeKind.Utc).AddTicks(6479),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 6, 1, 8, 41, 49, 892, DateTimeKind.Utc).AddTicks(9629));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 5, 25, 10, 31, 11, 16, DateTimeKind.Utc).AddTicks(5341),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 6, 1, 8, 41, 49, 892, DateTimeKind.Utc).AddTicks(8204));

            migrationBuilder.AddForeignKey(
                name: "FK_comments_comments_parent_comment_id",
                table: "comments",
                column: "parent_comment_id",
                principalTable: "comments",
                principalColumn: "id");
        }
    }
}
