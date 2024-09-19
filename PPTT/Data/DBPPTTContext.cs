using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PPTT.Models;

namespace PPTT.Data
{
    public class DBPPTTContext : DbContext
    {
        // Serctor instanciación //
        public DBPPTTContext(DbContextOptions<DBPPTTContext> options) : base(options)
        { }
        public DbSet<PPTT.Models.Admin> Usuarios { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<Servicio> Servicios { get; set; }

        // Serctor funciones //
        public async Task<List<Division>> GetDivisionAsync()
        {
            return await Divisions.FromSqlRaw("EXEC [dbo].[Retorna_Division]").ToListAsync();
        }
        public async Task<List<Servicio>> GetServiciosAsync(int division)
        {
            return await Servicios.FromSqlRaw("EXEC [dbo].[SelectServicios] @p0", division).ToListAsync();
        }                


        // Se especifica los Identity Keys de las entidades //       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          modelBuilder.Entity<Division>()
          .HasKey(d => d.ID_Division);

           modelBuilder.Entity<Servicio>()
          .HasKey(d => d.ID_Servicio);
        }
    }
}
