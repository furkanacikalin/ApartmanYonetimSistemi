using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApartmanYonetimSistemi.Migrations.Flat
{
    /// <inheritdoc />
    public partial class FlatInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Flats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApartmentId = table.Column<int>(type: "INTEGER", nullable: false),
                    FlatNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    ResidentUserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flats", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Flats");
        }
    }
}
