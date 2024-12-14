using five_birds_be.Models;
using Microsoft.EntityFrameworkCore;

namespace five_birds_be.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> User { get; set; }

        public DbSet<Candidate> Candidates { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
        
        }
    }

}