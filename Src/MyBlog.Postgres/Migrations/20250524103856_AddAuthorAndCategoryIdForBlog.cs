using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBlog.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorAndCategoryIdForBlog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "subscriptions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 5, 24, 10, 38, 54, 520, DateTimeKind.Utc).AddTicks(2090),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 5, 16, 13, 21, 32, 653, DateTimeKind.Utc).AddTicks(8761));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 5, 24, 10, 38, 54, 520, DateTimeKind.Utc).AddTicks(961),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 5, 16, 13, 21, 32, 653, DateTimeKind.Utc).AddTicks(7502));

            migrationBuilder.AddColumn<Guid>(
                name: "author_id",
                table: "blogs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "category_id",
                table: "blogs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "author_id",
                table: "blogs");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "blogs");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "subscriptions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 5, 16, 13, 21, 32, 653, DateTimeKind.Utc).AddTicks(8761),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 5, 24, 10, 38, 54, 520, DateTimeKind.Utc).AddTicks(2090));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 5, 16, 13, 21, 32, 653, DateTimeKind.Utc).AddTicks(7502),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 5, 24, 10, 38, 54, 520, DateTimeKind.Utc).AddTicks(961));
        }
    }
}
