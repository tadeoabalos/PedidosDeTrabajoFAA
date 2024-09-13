using Microsoft.EntityFrameworkCore;

namespace PPTT.Data
{
    public class DBPPTTContext : DbContext
    {
        public DBPPTTContext(DbContextOptions<DBPPTTContext> options)
            : base(options)
        { 
        }
        public DbSet<PPTT.Models.Admin> Admins { get; set; }         
    }
}
