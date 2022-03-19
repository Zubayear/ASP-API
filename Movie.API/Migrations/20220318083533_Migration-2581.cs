using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Movie.API.Migrations
{
    public partial class Migration2581 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ACTORS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ACTORS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MOVIES",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReleaseYear = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MOVIES", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ACTORS",
                columns: new[] { "Id", "Age", "Name" },
                values: new object[] { new Guid("7b89fb33-ee3b-49b6-ba33-8c4cd7b43c52"), 47, "Joaquin Phoenix" });

            migrationBuilder.InsertData(
                table: "ACTORS",
                columns: new[] { "Id", "Age", "Name" },
                values: new object[] { new Guid("cd53aed3-574d-403e-9726-81565f546023"), 78, "Robert De Niro" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ACTORS");

            migrationBuilder.DropTable(
                name: "MOVIES");
        }
    }
}
