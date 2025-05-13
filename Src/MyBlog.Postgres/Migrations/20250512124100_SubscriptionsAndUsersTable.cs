using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBlog.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class SubscriptionsAndUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(name: "IsDeleted", table: "tags", newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "comments",
                newName: "is_deleted"
            );

            migrationBuilder.RenameColumn(name: "IsDeleted", table: "blogs", newName: "is_deleted");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_name = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    normalize_user_name = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    email = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: false
                    ),
                    normalize_email = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: false
                    ),
                    password = table.Column<string>(type: "text", nullable: false),
                    avatar = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "subscriptions",
                columns: table => new
                {
                    follower_id = table.Column<Guid>(type: "uuid", nullable: false),
                    followed_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false,
                        defaultValue: new DateTime(
                            2025,
                            5,
                            12,
                            12,
                            40,
                            59,
                            688,
                            DateTimeKind.Utc
                        ).AddTicks(743)
                    ),
                    updated_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false,
                        defaultValue: new DateTime(
                            2025,
                            5,
                            12,
                            12,
                            40,
                            59,
                            688,
                            DateTimeKind.Utc
                        ).AddTicks(1800)
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscriptions", x => new { x.follower_id, x.followed_id });
                    table.ForeignKey(
                        name: "FK_subscriptions_users_followed_id",
                        column: x => x.followed_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_subscriptions_users_follower_id",
                        column: x => x.follower_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_followed_id",
                table: "subscriptions",
                column: "followed_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_users_normalize_email",
                table: "users",
                column: "normalize_email",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_users_normalize_user_name",
                table: "users",
                column: "normalize_user_name",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "subscriptions");

            migrationBuilder.DropTable(name: "users");

            migrationBuilder.RenameColumn(name: "is_deleted", table: "tags", newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "comments",
                newName: "IsDeleted"
            );

            migrationBuilder.RenameColumn(name: "is_deleted", table: "blogs", newName: "IsDeleted");
        }
    }
}
