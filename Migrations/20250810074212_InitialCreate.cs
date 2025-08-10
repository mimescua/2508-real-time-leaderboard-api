using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace _2508_real_time_leaderboard_api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "games",
                columns: table => new
                {
                    GameId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    About = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Platforms = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Genres = table.Column<int>(type: "int", nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_games", x => x.GameId);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(72)", maxLength: 72, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "scores",
                columns: table => new
                {
                    ScoreId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<int>(type: "int", nullable: false),
                    AchievedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scores", x => x.ScoreId);
                    table.ForeignKey(
                        name: "FK_scores_games_GameId",
                        column: x => x.GameId,
                        principalTable: "games",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_scores_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "games",
                columns: new[] { "GameId", "About", "Genres", "Platforms", "ReleaseDate", "Title" },
                values: new object[,]
                {
                    { 1, "An action-adventure game set in the fantasy land of Hyrule.", 331, "Nintendo 64, GameCube, iQue player", new DateTime(1998, 11, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Legend of Zelda: Ocarina of Time" },
                    { 2, "An open-world action-adventure game set in the fictional state of San Andreas.", 3459, "PlayStation 3, PlayStation 4, PlayStation 5, Xbox 360, Xbox One, PC", new DateTime(2013, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Grand Theft Auto V" }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "UserId", "CreatedAt", "Email", "Password", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@leaderboard.com", "admin123", "admin" },
                    { 2, new DateTime(2024, 10, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "user1@leaderboard.com", "user123", "user1" }
                });

            migrationBuilder.InsertData(
                table: "scores",
                columns: new[] { "ScoreId", "AchievedAt", "GameId", "UserId", "Value" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 100 },
                    { 2, new DateTime(2025, 10, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1, 900 },
                    { 3, new DateTime(2025, 10, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2, 600 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_scores_GameId",
                table: "scores",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_scores_UserId_GameId",
                table: "scores",
                columns: new[] { "UserId", "GameId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_Username",
                table: "users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "scores");

            migrationBuilder.DropTable(
                name: "games");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
