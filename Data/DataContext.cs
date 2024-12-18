using five_birds_be.Models;
using Microsoft.EntityFrameworkCore;

namespace five_birds_be.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> User { get; set; }
        public DbSet<Exam> Exam { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<Answer> Answer { get; set; }
        public DbSet<Result> Result { get; set; }


        public DbSet<Candidate> Candidates { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
        builder.Entity<Exam>()
        .HasMany(e => e.Question)
        .WithOne(e => e.Exam)
        .HasForeignKey(e => e.ExamId);

        builder.Entity<Question>()
        .HasMany(e => e.Answers)
        .WithOne(e => e.Question)
        .HasForeignKey(e => e.QuestionId);

        }
    }

}