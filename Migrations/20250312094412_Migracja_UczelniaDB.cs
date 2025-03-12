using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC_EduScanner.Migrations
{
    /// <inheritdoc />
    public partial class Migracja_UczelniaDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Prowadzacy",
                columns: table => new
                {
                    ProwadzacyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Imie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nazwisko = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prowadzacy", x => x.ProwadzacyID);
                });

            migrationBuilder.CreateTable(
                name: "Przedmioty",
                columns: table => new
                {
                    PrzedmiotID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NazwaPrzedmiotu = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Przedmioty", x => x.PrzedmiotID);
                });

            migrationBuilder.CreateTable(
                name: "PrzedmiotProwadzacy",
                columns: table => new
                {
                    PrzedmiotID = table.Column<int>(type: "int", nullable: false),
                    ProwadzacyID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrzedmiotProwadzacy", x => new { x.PrzedmiotID, x.ProwadzacyID });
                    table.ForeignKey(
                        name: "FK_PrzedmiotProwadzacy_Prowadzacy_ProwadzacyID",
                        column: x => x.ProwadzacyID,
                        principalTable: "Prowadzacy",
                        principalColumn: "ProwadzacyID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrzedmiotProwadzacy_Przedmioty_PrzedmiotID",
                        column: x => x.PrzedmiotID,
                        principalTable: "Przedmioty",
                        principalColumn: "PrzedmiotID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrzedmiotProwadzacy_ProwadzacyID",
                table: "PrzedmiotProwadzacy",
                column: "ProwadzacyID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrzedmiotProwadzacy");

            migrationBuilder.DropTable(
                name: "Prowadzacy");

            migrationBuilder.DropTable(
                name: "Przedmioty");
        }
    }
}
