using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StsKlassifikation.Migrations
{
    public partial class initialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Klassifikation",
                columns: table => new
                {
                    UUID = table.Column<string>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: true),
                    Livscykluskode = table.Column<string>(nullable: true),
                    BrugervendtNoegle = table.Column<string>(nullable: true),
                    Titel = table.Column<string>(nullable: true),
                    Beskrivelse = table.Column<string>(nullable: true),
                    Synkroniser = table.Column<bool>(nullable: false),
                    Ansvarlig = table.Column<string>(nullable: true),
                    Ejer = table.Column<string>(nullable: true),
                    Publiceret = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Klassifikation", x => x.UUID);
                });

            migrationBuilder.CreateTable(
                name: "Facet",
                columns: table => new
                {
                    UUID = table.Column<string>(nullable: false),
                    BrugervendtNoegle = table.Column<string>(nullable: true),
                    Titel = table.Column<string>(nullable: true),
                    Beskrivelse = table.Column<string>(nullable: true),
                    Opbygning = table.Column<string>(nullable: true),
                    Ophavsret = table.Column<string>(nullable: true),
                    PlanIdentifikator = table.Column<string>(nullable: true),
                    SupplementTekst = table.Column<string>(nullable: true),
                    RetskildeTekst = table.Column<string>(nullable: true),
                    Livscykluskode = table.Column<string>(nullable: true),
                    Publiceret = table.Column<bool>(nullable: true),
                    Ansvarlig = table.Column<string>(nullable: true),
                    Ejer = table.Column<string>(nullable: true),
                    FacetTilhoerer = table.Column<string>(nullable: true),
                    KlassifikationUUID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facet", x => x.UUID);
                    table.ForeignKey(
                        name: "FK_Facet_Klassifikation_KlassifikationUUID",
                        column: x => x.KlassifikationUUID,
                        principalTable: "Klassifikation",
                        principalColumn: "UUID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FacetRedaktoer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: true),
                    FacetUUID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacetRedaktoer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacetRedaktoer_Facet_FacetUUID",
                        column: x => x.FacetUUID,
                        principalTable: "Facet",
                        principalColumn: "UUID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Klasse",
                columns: table => new
                {
                    UUID = table.Column<string>(nullable: false),
                    BrugervendtNoegle = table.Column<string>(nullable: true),
                    Titel = table.Column<string>(nullable: true),
                    Beskrivelse = table.Column<string>(nullable: true),
                    Omfang = table.Column<string>(nullable: true),
                    Retskilde = table.Column<string>(nullable: true),
                    Aendringsnotat = table.Column<string>(nullable: true),
                    Livscykluskode = table.Column<string>(nullable: true),
                    Tilstand = table.Column<bool>(nullable: true),
                    Ansvarlig = table.Column<string>(nullable: true),
                    Ejer = table.Column<string>(nullable: true),
                    Soegeord = table.Column<string>(nullable: true),
                    FacetUUID = table.Column<string>(nullable: true),
                    OverordnetKlasse = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Klasse", x => x.UUID);
                    table.ForeignKey(
                        name: "FK_Klasse_Facet_FacetUUID",
                        column: x => x.FacetUUID,
                        principalTable: "Facet",
                        principalColumn: "UUID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KlasseErstatter",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: true),
                    KlasseUUID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KlasseErstatter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KlasseErstatter_Klasse_KlasseUUID",
                        column: x => x.KlasseUUID,
                        principalTable: "Klasse",
                        principalColumn: "UUID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KlasseLovligeKombination",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: true),
                    KlasseUUID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KlasseLovligeKombination", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KlasseLovligeKombination_Klasse_KlasseUUID",
                        column: x => x.KlasseUUID,
                        principalTable: "Klasse",
                        principalColumn: "UUID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KlasseRedaktoer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: true),
                    KlasseUUID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KlasseRedaktoer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KlasseRedaktoer_Klasse_KlasseUUID",
                        column: x => x.KlasseUUID,
                        principalTable: "Klasse",
                        principalColumn: "UUID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KlasseSideordnet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: true),
                    KlasseUUID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KlasseSideordnet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KlasseSideordnet_Klasse_KlasseUUID",
                        column: x => x.KlasseUUID,
                        principalTable: "Klasse",
                        principalColumn: "UUID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KlasseTilfoejelse",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: true),
                    KlasseUUID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KlasseTilfoejelse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KlasseTilfoejelse_Klasse_KlasseUUID",
                        column: x => x.KlasseUUID,
                        principalTable: "Klasse",
                        principalColumn: "UUID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Facet_KlassifikationUUID",
                table: "Facet",
                column: "KlassifikationUUID");

            migrationBuilder.CreateIndex(
                name: "IX_FacetRedaktoer_FacetUUID",
                table: "FacetRedaktoer",
                column: "FacetUUID");

            migrationBuilder.CreateIndex(
                name: "IX_Klasse_FacetUUID",
                table: "Klasse",
                column: "FacetUUID");

            migrationBuilder.CreateIndex(
                name: "IX_KlasseErstatter_KlasseUUID",
                table: "KlasseErstatter",
                column: "KlasseUUID");

            migrationBuilder.CreateIndex(
                name: "IX_KlasseLovligeKombination_KlasseUUID",
                table: "KlasseLovligeKombination",
                column: "KlasseUUID");

            migrationBuilder.CreateIndex(
                name: "IX_KlasseRedaktoer_KlasseUUID",
                table: "KlasseRedaktoer",
                column: "KlasseUUID");

            migrationBuilder.CreateIndex(
                name: "IX_KlasseSideordnet_KlasseUUID",
                table: "KlasseSideordnet",
                column: "KlasseUUID");

            migrationBuilder.CreateIndex(
                name: "IX_KlasseTilfoejelse_KlasseUUID",
                table: "KlasseTilfoejelse",
                column: "KlasseUUID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FacetRedaktoer");

            migrationBuilder.DropTable(
                name: "KlasseErstatter");

            migrationBuilder.DropTable(
                name: "KlasseLovligeKombination");

            migrationBuilder.DropTable(
                name: "KlasseRedaktoer");

            migrationBuilder.DropTable(
                name: "KlasseSideordnet");

            migrationBuilder.DropTable(
                name: "KlasseTilfoejelse");

            migrationBuilder.DropTable(
                name: "Klasse");

            migrationBuilder.DropTable(
                name: "Facet");

            migrationBuilder.DropTable(
                name: "Klassifikation");
        }
    }
}
