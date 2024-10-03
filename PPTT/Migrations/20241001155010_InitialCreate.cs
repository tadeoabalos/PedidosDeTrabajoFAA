using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPTT.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_usuario",
                table: "usuario");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "usuario");

            migrationBuilder.RenameTable(
                name: "usuario",
                newName: "Usuario");

            migrationBuilder.RenameColumn(
                name: "Dni",
                table: "Usuario",
                newName: "DNI");

            migrationBuilder.RenameColumn(
                name: "Sname",
                table: "Usuario",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "Servicio",
                table: "Usuario",
                newName: "Correo");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Usuario",
                newName: "Apellido");

            migrationBuilder.RenameColumn(
                name: "Division",
                table: "Usuario",
                newName: "ID_Servicio_Fk");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Usuario",
                newName: "ID_Usuario_Pk");

            migrationBuilder.AlterColumn<string>(
                name: "Numero_Control",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "Fecha_Alta",
                table: "Usuario",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Fecha_Baja",
                table: "Usuario",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ID_Division_Fk",
                table: "Usuario",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ID_Password_Fk",
                table: "Usuario",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ID_Rol_Fk",
                table: "Usuario",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Usuario",
                table: "Usuario",
                column: "ID_Usuario_Pk");

            migrationBuilder.CreateTable(
                name: "DependenciaInterna",
                columns: table => new
                {
                    ID_Dependencia_Interna_PK = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Organismo_FK = table.Column<int>(type: "int", nullable: false),
                    Descripcion_Dependencia = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DependenciaInterna", x => x.ID_Dependencia_Interna_PK);
                });

            migrationBuilder.CreateTable(
                name: "Divisions",
                columns: table => new
                {
                    ID_Division_Pk = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion_Division = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Divisions", x => x.ID_Division_Pk);
                });

            migrationBuilder.CreateTable(
                name: "Estado",
                columns: table => new
                {
                    ID_Estado_PK = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion_Estado = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estado", x => x.ID_Estado_PK);
                });

            migrationBuilder.CreateTable(
                name: "grado",
                columns: table => new
                {
                    ID_Grado_PK = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion_Grado = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grado", x => x.ID_Grado_PK);
                });

            migrationBuilder.CreateTable(
                name: "orden_de_trabajo_usuario",
                columns: table => new
                {
                    ID_Orden_De_Trabajo_Pk = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha_Subida = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IP_Solicitante = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nombre_Solicitante = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Apellido_Solicitante = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ID_Grado_Fk = table.Column<int>(type: "int", nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ID_Organismo_Fk = table.Column<int>(type: "int", nullable: false),
                    RTI_Solicitante = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color_Oficina_PT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Numero_Oficina_PT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Piso_Oficina_PT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color_Oficina_Solicitante = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Numero_Oficina_Solicitante = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Piso_Oficina_Solicitante = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ID_Dependencia_Interna_Fk = table.Column<int>(type: "int", nullable: false),
                    DNI_Solicitante = table.Column<int>(type: "int", nullable: false),
                    Numero_Control_Solicitante = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fecha_Estimada_Fin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Fecha_Inicio_Suspendido = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Prioridad = table.Column<int>(type: "int", nullable: false),
                    ID_Estado_Fk = table.Column<int>(type: "int", nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orden_de_trabajo_usuario", x => x.ID_Orden_De_Trabajo_Pk);
                });

            migrationBuilder.CreateTable(
                name: "Organismo",
                columns: table => new
                {
                    ID_Organismo_PK = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion_organismo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organismo", x => x.ID_Organismo_PK);
                });

            migrationBuilder.CreateTable(
                name: "Prueba",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prueba", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Servicios",
                columns: table => new
                {
                    ID_Servicio_Pk = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion_Servicio = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicios", x => x.ID_Servicio_Pk);
                });

            migrationBuilder.CreateTable(
                name: "Tarea",
                columns: table => new
                {
                    Id_Tarea_Pk = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion_Tarea = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Id_Servicio_Fk = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarea", x => x.Id_Tarea_Pk);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DependenciaInterna");

            migrationBuilder.DropTable(
                name: "Divisions");

            migrationBuilder.DropTable(
                name: "Estado");

            migrationBuilder.DropTable(
                name: "grado");

            migrationBuilder.DropTable(
                name: "orden_de_trabajo_usuario");

            migrationBuilder.DropTable(
                name: "Organismo");

            migrationBuilder.DropTable(
                name: "Prueba");

            migrationBuilder.DropTable(
                name: "Servicios");

            migrationBuilder.DropTable(
                name: "Tarea");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Usuario",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "Fecha_Alta",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "Fecha_Baja",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "ID_Division_Fk",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "ID_Password_Fk",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "ID_Rol_Fk",
                table: "Usuario");

            migrationBuilder.RenameTable(
                name: "Usuario",
                newName: "usuario");

            migrationBuilder.RenameColumn(
                name: "DNI",
                table: "usuario",
                newName: "Dni");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "usuario",
                newName: "Sname");

            migrationBuilder.RenameColumn(
                name: "ID_Servicio_Fk",
                table: "usuario",
                newName: "Division");

            migrationBuilder.RenameColumn(
                name: "Correo",
                table: "usuario",
                newName: "Servicio");

            migrationBuilder.RenameColumn(
                name: "Apellido",
                table: "usuario",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ID_Usuario_Pk",
                table: "usuario",
                newName: "Id");

            migrationBuilder.AlterColumn<int>(
                name: "Numero_Control",
                table: "usuario",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "usuario",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_usuario",
                table: "usuario",
                column: "Id");
        }
    }
}
