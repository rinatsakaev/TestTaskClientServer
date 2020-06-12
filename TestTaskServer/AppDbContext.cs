using Microsoft.EntityFrameworkCore;
using System;

namespace TestTaskServer
{
    public class AppDbContext : DbContext
    {
        public DbSet<Word> Words { get; set; }
        private readonly string connectionString;

        public AppDbContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(connectionString, options => options.SetPostgresVersion(new Version(9, 6)));
        }
    }
}
