using Microsoft.EntityFrameworkCore.Migrations;

namespace StsKlassifikation.Migrations
{
    public partial class KlasseTilhoerer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KlasseTilhoerer",
                table: "Klasse",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KlasseTilhoerer",
                table: "Klasse");
        }
    }
}
