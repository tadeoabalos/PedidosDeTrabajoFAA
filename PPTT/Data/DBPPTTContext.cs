using System;
using System.Collections.Generic;
using System.Data;
using System.Formats.Asn1;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore;
using PPTT.Models;

namespace PPTT.Data
{
    public class DBPPTTContext : DbContext
    {
        // Serctor instanciación //
        public DBPPTTContext(DbContextOptions<DBPPTTContext> options) : base(options)
        { }
        public DbSet<PPTT.Models.Admin> Usuario { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<PTUsuario> PTUsuario { get; set; }
        public DbSet<Grado> Grados { get; set; }
        public DbSet<Organismo> Organismo { get; set; }
        public DbSet<Dependencia_Interna> Dependencia_Interna { get; set; }
        public DbSet<Estado> Estado { get; set; }
        public DbSet<Prueba> Prueba { get; set; }
        public DbSet<PTUsuario> PTUsuarios { get; set; }

        public DbSet<Tarea> Tarea { get; set; }
        public DbSet<Orden_Asignada> Orden_Asignada { get; set; }

        // Sector funciones //
        public async Task<List<Division>> GetDivisionAsync()
        {
            return await Divisions.FromSqlRaw("EXEC [dbo].[Retorna_Division]").ToListAsync();
        }
        public async Task<List<Servicio>> GetServiciosAsync(int division)
        {
            return await Servicios.FromSqlRaw("EXEC [dbo].[Servicios_Filtrados] @p0", division).ToListAsync();
        }
        public async Task<List<Grado>> GetGradosAsync()
        {
            return await Grados.FromSqlRaw("EXEC [dbo].[Retorna_Grado]").ToListAsync();
        }
        public async Task<List<Servicio>> GetServiciosSinFiltrarAsync()
        {
            return await Servicios.FromSqlRaw("EXEC [dbo].[Retorna_Servicios]").ToListAsync();
        }
        public async Task<List<Organismo>> GetOrganismoAsync()
        {
            return await Organismo.FromSqlRaw("EXEC [dbo].[Retorna_Organismo]").ToListAsync();
        }
        public async Task<List<Tarea>> GetTareasFiltradasAsync(int servicio)
        {
            return await Tarea.FromSqlRaw("EXEC [dbo].[Tareas_Filtradas] @p0", servicio).ToListAsync();
        }
        public async Task<List<Dependencia_Interna>> GetDependenciasFiltradasAsync(int organismo)
        {
            return await Dependencia_Interna.FromSqlRaw("EXEC [dbo].[Dependencia_Filtrada] @p0", organismo).ToListAsync();
        }
        public async Task<List<Admin>> GetUsuariosFiltradosAsync(int division)
        {
            return await Usuario.FromSqlRaw("EXEC [dbo].[Usuarios_Filtrados]  @p0", division).ToListAsync();
        }
        public async Task<List<Admin>> GetUsuariosAsync()
        {
            return await Usuario.FromSqlRaw("EXEC [dbo].[Retorna_Usuarios]").ToListAsync();
        }                

        // IDENTITYS
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           modelBuilder.Entity<PPTT.Models.Admin>()
          .HasKey(d => d.ID_Usuario_Pk);

           modelBuilder.Entity<Division>()
          .HasKey(d => d.ID_Division_Pk);

           modelBuilder.Entity<Servicio>()
          .HasKey(d => d.ID_Servicio_Pk);

           modelBuilder.Entity<Dependencia_Interna>()
          .HasKey(d => d.ID_Dependencia_Interna_PK);

           modelBuilder.Entity<Grado>()
          .HasKey(d => d.ID_Grado_PK);

           modelBuilder.Entity<Estado>()
           .HasKey(d => d.ID_Estado_PK);

           modelBuilder.Entity<Organismo>()
          .HasKey(d => d.ID_Organismo_PK);

            modelBuilder.Entity<PTUsuario>()
          .HasKey(d => d.ID_Orden_Trabajo_Pk);

            modelBuilder.Entity<Tarea>()
          .HasKey(d => d.Id_Tarea_Pk);

            modelBuilder.Entity<Prueba>()
            .HasKey(d => d.ID);

            modelBuilder.Entity<Orden_Asignada>()
            .HasKey(d => d.ID_Trabajo_Asignado_Pk);
        }
    }
}
