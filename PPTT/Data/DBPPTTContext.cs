using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PPTT.Models;

namespace PPTT.Data
{
    public class DBPPTTContext : DbContext
    {
        public DBPPTTContext(DbContextOptions<DBPPTTContext> options)
            : base(options)
        {
        }
        public DbSet<PPTT.Models.Admin> Usuarios { get; set; }
    }
}
