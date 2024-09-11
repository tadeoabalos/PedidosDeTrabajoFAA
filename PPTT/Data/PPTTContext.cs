using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PPTT.Models;

namespace PPTT.Data
{
    public class PPTTContext : DbContext
    {
        public PPTTContext (DbContextOptions<PPTTContext> options)
            : base(options)
        {
        }

        public DbSet<PPTT.Models.Admin> Admin { get; set; } = default!;
    }
}
