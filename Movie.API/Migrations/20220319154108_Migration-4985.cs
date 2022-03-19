using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Movie.API.Migrations
{
    public partial class Migration4985 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ACTORS",
                keyColumn: "Id",
                keyValue: new Guid("7b89fb33-ee3b-49b6-ba33-8c4cd7b43c52"));

            migrationBuilder.DeleteData(
                table: "ACTORS",
                keyColumn: "Id",
                keyValue: new Guid("cd53aed3-574d-403e-9726-81565f546023"));

            migrationBuilder.CreateTable(
                name: "ActorMovies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MovieId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActorMovies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActorMovies_ACTORS_ActorId",
                        column: x => x.ActorId,
                        principalTable: "ACTORS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActorMovies_MOVIES_MovieId",
                        column: x => x.MovieId,
                        principalTable: "MOVIES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActorMovies_ActorId",
                table: "ActorMovies",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_ActorMovies_MovieId",
                table: "ActorMovies",
                column: "MovieId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActorMovies");

            migrationBuilder.InsertData(
                table: "ACTORS",
                columns: new[] { "Id", "Age", "Name" },
                values: new object[] { new Guid("7b89fb33-ee3b-49b6-ba33-8c4cd7b43c52"), 47, "Joaquin Phoenix" });

            migrationBuilder.InsertData(
                table: "ACTORS",
                columns: new[] { "Id", "Age", "Name" },
                values: new object[] { new Guid("cd53aed3-574d-403e-9726-81565f546023"), 78, "Robert De Niro" });
        }
    }
}
