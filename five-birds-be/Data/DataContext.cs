using five_birds_be.Models;
using Microsoft.EntityFrameworkCore;

namespace five_birds_be.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<TrustedDevice> TrustedDevices { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
        
        }
    }

}