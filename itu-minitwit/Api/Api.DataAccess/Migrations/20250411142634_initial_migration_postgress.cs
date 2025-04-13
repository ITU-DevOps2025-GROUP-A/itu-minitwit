using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Api.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class initial_migration_postgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "follower",
                columns: table => new
                {
                    who_id = table.Column<int>(type: "integer", nullable: false),
                    whom_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_follower", x => new { x.who_id, x.whom_id });
                });

            migrationBuilder.CreateTable(
                name: "LatestProcessedSimActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Latest = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LatestProcessedSimActions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    pw_hash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "message",
                columns: table => new
                {
                    message_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    author_id = table.Column<int>(type: "integer", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false),
                    pub_date = table.Column<int>(type: "integer", nullable: true),
                    flagged = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_message", x => x.message_id);
                    table.ForeignKey(
                        name: "FK_message_user_author_id",
                        column: x => x.author_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_follower_who_id_whom_id",
                table: "follower",
                columns: new[] { "who_id", "whom_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_message_author_id_flagged_pub_date",
                table: "message",
                columns: new[] { "author_id", "flagged", "pub_date" },
                descending: new[] { false, false, true });

            migrationBuilder.CreateIndex(
                name: "IX_message_flagged_pub_date",
                table: "message",
                columns: new[] { "flagged", "pub_date" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_user_username",
                table: "user",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "follower");

            migrationBuilder.DropTable(
                name: "LatestProcessedSimActions");

            migrationBuilder.DropTable(
                name: "message");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
