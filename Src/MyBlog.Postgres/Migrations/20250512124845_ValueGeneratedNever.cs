using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBlog.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class ValueGeneratedNever : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "subscriptions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 5, 12, 12, 48, 45, 165, DateTimeKind.Utc).AddTicks(
                    5087
                ),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(
                    2025,
                    5,
                    12,
                    12,
                    40,
                    59,
                    688,
                    DateTimeKind.Utc
                ).AddTicks(1800)
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 5, 12, 12, 48, 45, 165, DateTimeKind.Utc).AddTicks(
                    4385
                ),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(
                    2025,
                    5,
                    12,
                    12,
                    40,
                    59,
                    688,
                    DateTimeKind.Utc
                ).AddTicks(743)
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "subscriptions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 5, 12, 12, 40, 59, 688, DateTimeKind.Utc).AddTicks(
                    1800
                ),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(
                    2025,
                    5,
                    12,
                    12,
                    48,
                    45,
                    165,
                    DateTimeKind.Utc
                ).AddTicks(5087)
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 5, 12, 12, 40, 59, 688, DateTimeKind.Utc).AddTicks(
                    743
                ),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(
                    2025,
                    5,
                    12,
                    12,
                    48,
                    45,
                    165,
                    DateTimeKind.Utc
                ).AddTicks(4385)
            );
        }
    }
}
