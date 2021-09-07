using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

namespace Redis.Migrations
{
    public partial class baseMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KorisnikIgras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    IdKorisnik = table.Column<int>(type: "int", nullable: false),
                    IdIgra = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KorisnikIgras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Igras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Naziv = table.Column<string>(type: "text", nullable: true),
                    Zanr = table.Column<string>(type: "text", nullable: true),
                    Cijena = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    KorisnikIgraId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Igras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Igras_KorisnikIgras_KorisnikIgraId",
                        column: x => x.KorisnikIgraId,
                        principalTable: "KorisnikIgras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Korisniks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(type: "text", nullable: true),
                    Lozinka = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Uloga = table.Column<string>(type: "text", nullable: true),
                    KorisnikIgraId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Korisniks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Korisniks_KorisnikIgras_KorisnikIgraId",
                        column: x => x.KorisnikIgraId,
                        principalTable: "KorisnikIgras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Igras_KorisnikIgraId",
                table: "Igras",
                column: "KorisnikIgraId");

            migrationBuilder.CreateIndex(
                name: "IX_Korisniks_KorisnikIgraId",
                table: "Korisniks",
                column: "KorisnikIgraId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Igras");

            migrationBuilder.DropTable(
                name: "Korisniks");

            migrationBuilder.DropTable(
                name: "KorisnikIgras");
        }
    }
}
