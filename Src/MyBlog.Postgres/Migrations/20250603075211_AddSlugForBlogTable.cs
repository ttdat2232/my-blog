using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBlog.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddSlugForBlogTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "subscriptions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 6, 3, 7, 52, 8, 592, DateTimeKind.Utc).AddTicks(6772),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 6, 1, 8, 41, 49, 892, DateTimeKind.Utc).AddTicks(9629));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 6, 3, 7, 52, 8, 592, DateTimeKind.Utc).AddTicks(5465),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 6, 1, 8, 41, 49, 892, DateTimeKind.Utc).AddTicks(8204));

            migrationBuilder.AddColumn<string>(
                name: "slug",
                table: "blogs",
                type: "character varying(400)",
                maxLength: 400,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "slug",
                table: "blogs");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "subscriptions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 6, 1, 8, 41, 49, 892, DateTimeKind.Utc).AddTicks(9629),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 6, 3, 7, 52, 8, 592, DateTimeKind.Utc).AddTicks(6772));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 6, 1, 8, 41, 49, 892, DateTimeKind.Utc).AddTicks(8204),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 6, 3, 7, 52, 8, 592, DateTimeKind.Utc).AddTicks(5465));
        }
    }
}
