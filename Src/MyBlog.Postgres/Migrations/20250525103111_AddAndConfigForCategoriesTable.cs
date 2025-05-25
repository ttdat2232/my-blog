using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBlog.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddAndConfigForCategoriesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "subscriptions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 5, 25, 10, 31, 11, 16, DateTimeKind.Utc).AddTicks(6479),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 5, 24, 10, 38, 54, 520, DateTimeKind.Utc).AddTicks(2090));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 5, 25, 10, 31, 11, 16, DateTimeKind.Utc).AddTicks(5341),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 5, 24, 10, 38, 54, 520, DateTimeKind.Utc).AddTicks(961));

            migrationBuilder.AlterColumn<Guid>(
                name: "category_id",
                table: "blogs",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "author_id",
                table: "blogs",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "TEXT", nullable: true),
                    normalize_name = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_blogs_author_id",
                table: "blogs",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_blogs_category_id",
                table: "blogs",
                column: "category_id");

            migrationBuilder.AddForeignKey(
                name: "FK_blogs_categories_category_id",
                table: "blogs",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_blogs_users_author_id",
                table: "blogs",
                column: "author_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_blogs_categories_category_id",
                table: "blogs");

            migrationBuilder.DropForeignKey(
                name: "FK_blogs_users_author_id",
                table: "blogs");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropIndex(
                name: "IX_blogs_author_id",
                table: "blogs");

            migrationBuilder.DropIndex(
                name: "IX_blogs_category_id",
                table: "blogs");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "subscriptions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 5, 24, 10, 38, 54, 520, DateTimeKind.Utc).AddTicks(2090),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 5, 25, 10, 31, 11, 16, DateTimeKind.Utc).AddTicks(6479));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 5, 24, 10, 38, 54, 520, DateTimeKind.Utc).AddTicks(961),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 5, 25, 10, 31, 11, 16, DateTimeKind.Utc).AddTicks(5341));

            migrationBuilder.AlterColumn<Guid>(
                name: "category_id",
                table: "blogs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "author_id",
                table: "blogs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
