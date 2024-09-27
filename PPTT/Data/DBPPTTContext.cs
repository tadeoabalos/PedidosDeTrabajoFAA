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
        public DbSet<PPTT.Models.Admin> usuario { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<Servicio> Servicios { get; set; }

        // Sector funciones //
        public async Task<List<Division>> GetDivisionAsync()
        {
            return await Divisions.FromSqlRaw("EXEC [dbo].[Retorna_Division]").ToListAsync();
        }
        public async Task<List<Servicio>> GetServiciosAsync(int division)
        {
            return await Servicios.FromSqlRaw("EXEC [dbo].[Servicios_Filtrados] @p0", division).ToListAsync();
        }                

        // Se especifica los Identity Keys de las entidades //
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           modelBuilder.Entity<PPTT.Models.Admin>()
          .HasKey(d => d.ID_Usuario_Pk);

            modelBuilder.Entity<Division>()
          .HasKey(d => d.ID_Division_Pk);

           modelBuilder.Entity<Servicio>()
          .HasKey(d => d.ID_Servicio_Pk);
        }
    }
}
