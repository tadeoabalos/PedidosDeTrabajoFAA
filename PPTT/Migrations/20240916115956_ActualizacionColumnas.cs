using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPTT.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionColumnas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dni = table.Column<int>(type: "int", nullable: false),
                    NumeroControl = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Division = table.Column<int>(type: "int", nullable: true),
                    Servicio = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
