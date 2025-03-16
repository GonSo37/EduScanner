using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC_EduScanner.Migrations
{
    /// <inheritdoc />
    public partial class EduScanner_migrationV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Format",
                table: "Przedmioty",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tytul",
                table: "Prowadzacy",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Format",
                table: "Przedmioty");

            migrationBuilder.DropColumn(
                name: "Tytul",
                table: "Prowadzacy");
        }
    }
}
