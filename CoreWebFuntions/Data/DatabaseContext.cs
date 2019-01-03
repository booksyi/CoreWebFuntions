using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebFuntions.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        /*
        public DbSet<Models.Article> Articles { get; set; }
        public DbSet<Models.Epaper> Epapers { get; set; }
        public DbSet<Models.Member> Members { get; set; }
        */

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.ApplyConfiguration(new SystemConstantModelConfiguration());
        }
    }
}
