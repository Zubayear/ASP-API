using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Movie.API.Migrations
{
    public partial class Migration29286 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActorMovies_ACTORS_ActorId",
                table: "ActorMovies");

            migrationBuilder.DropForeignKey(
                name: "FK_ActorMovies_MOVIES_MovieId",
                table: "ActorMovies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MOVIES",
                table: "MOVIES");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ACTORS",
                table: "ACTORS");

            migrationBuilder.RenameTable(
                name: "MOVIES",
                newName: "Movies");

            migrationBuilder.RenameTable(
                name: "ACTORS",
                newName: "Actors");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Movies",
                table: "Movies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Actors",
                table: "Actors",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActorMovies_Actors_ActorId",
                table: "ActorMovies",
                column: "ActorId",
                principalTable: "Actors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActorMovies_Movies_MovieId",
                table: "ActorMovies",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActorMovies_Actors_ActorId",
                table: "ActorMovies");

            migrationBuilder.DropForeignKey(
                name: "FK_ActorMovies_Movies_MovieId",
                table: "ActorMovies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Movies",
                table: "Movies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Actors",
                table: "Actors");

            migrationBuilder.RenameTable(
                name: "Movies",
                newName: "MOVIES");

            migrationBuilder.RenameTable(
                name: "Actors",
                newName: "ACTORS");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MOVIES",
                table: "MOVIES",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ACTORS",
                table: "ACTORS",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActorMovies_ACTORS_ActorId",
                table: "ActorMovies",
                column: "ActorId",
                principalTable: "ACTORS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActorMovies_MOVIES_MovieId",
                table: "ActorMovies",
                column: "MovieId",
                principalTable: "MOVIES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
